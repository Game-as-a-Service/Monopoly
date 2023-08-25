using Domain.Common;
using Domain.Events;

namespace Domain;

public abstract class Block
{
    public string Id { get; }
    public Block? Up { get; set; }
    public Block? Down { get; set; }
    public Block? Left { get; set; }
    public Block? Right { get; set; }
    protected string lot = "";
    public string Lot => lot;

    public List<Map.Direction> Directions => new List<Map.Direction>()
    {
        Up is not null ? Map.Direction.Up : Map.Direction.None,
        Down is not null ? Map.Direction.Down : Map.Direction.None,
        Left is not null ? Map.Direction.Left : Map.Direction.None,
        Right is not null ? Map.Direction.Right : Map.Direction.None
    }.Where(d => d is not Map.Direction.None).ToList();

    public Block(string id)
    {
        Id = id;
    }

    public Block? GetDirectionBlock(Map.Direction d)
    {
        return d switch
        {
            Map.Direction.Up => Up,
            Map.Direction.Down => Down,
            Map.Direction.Left => Left,
            Map.Direction.Right => Right,
            _ => throw new ArgumentOutOfRangeException(nameof(d), d, null)
        };
    }

    public virtual Player? GetOwner()
    {
        return null;
    }

    public virtual void UpdateOwner(Player Owner)
    {
        throw new Exception("此地不可購買！");
    }
}

public class Land : Block
{
    private static readonly decimal[] RATE_OF_HOUSE = new decimal[] { 0.05m, 0.4m, 1, 3, 6, 10 };
    private static readonly decimal[] RATE_OF_LOT = new decimal[] { 0, 1, 1.3m, 2, 4, 8, 16 };
    private static readonly int MAX_HOUSE = 5;

    protected LandContract landContract;
    protected readonly decimal _price;
    protected int _house = 0;

    public decimal Price => _price; // 土地購買價格

    public decimal UpgradePrice => _price; // 升級價格

    // public int TollFee => _price; // 過路費
    public int House => _house;

    public Land(string id, decimal price = 1000, string lot = " ") : base(id)
    {
        _price = price;
        landContract = new LandContract(null, this);
        this.lot = lot;
    }

    public virtual void Upgrade()
    {
        _house++;
    }

    public DomainEvent PayToll(Player payer)
    {
        // payer 應該付過路費給 payee
        // 計算過路費

        Player? payee = landContract.Owner;

        if (payer.EndRoundFlag)
        {
            //throw new Exception("玩家不需要支付過路費");
            return new PlayerDoesntNeedToPayTollEvent(payer.Monopoly.Id, payer.Id, payer.Money);
        }
        else if (payee!.Chess.CurrentBlock.Id == "Jail" || payee.Chess.CurrentBlock.Id == "ParkingLot")
        {
            //throw new Exception("不需要支付過路費：Owner is in the " + payee.Chess.CurrentBlock.Id);
            return new PlayerDoesntNeedToPayTollEvent(payer.Monopoly.Id, payer.Id, payer.Money);
        }
        else
        {
            decimal amount = CalcullateToll(payee);

            if (payer.Money > amount)
            {
                payer.EndRoundFlag = true;
                payer.PayToll(payee, amount);
                return new PlayerPayTollEvent(payer.Monopoly.Id, payer.Id, payer.Money, payee.Id, payee.Money);
            }
            else
            {
                //throw new Exception("錢包餘額不足！");
                return new PlayerTooPoorToPayTollEvent(payer.Monopoly.Id, payer.Id, payer.Money, amount);
            }
        }
    }

    public virtual decimal CalcullateToll(Player payee)
    {
        int lotCount = payee.LandContractList.Count(t => t.Land.Lot == lot);

        return _price * RATE_OF_HOUSE[_house] * RATE_OF_LOT[lotCount];
    }

    public decimal GetPrice(string use)
    {
        return use switch
        {
            "Mortgage" or "UnSold" => _price * (1 + _house) * (decimal)0.7,
            "Redeem" => _price * (1 + _house),
            "Auction" => _price * (1 + _house) * (decimal)0.5,
            _ => _price * (1 + _house),
        };
    }

    public override Player? GetOwner()
    {
        return landContract.Owner;
    }

    public override void UpdateOwner(Player Owner)
    {
        landContract.Owner = Owner;
    }

    public virtual DomainEvent BuildHouse(Player player)
    {
        if (GetOwner() == player)
        {
            if (_house == MAX_HOUSE) return new HouseMaxEvent(player.Monopoly.Id, player.Id, Id, _house);

            if (UpgradePrice <= player.Money)
            {
                player.Money -= UpgradePrice;
                _house++;
                player.EnableUpgrade = false;
                return new PlayerBuildHouseEvent(player.Monopoly.Id, player.Id, Id, player.Money, _house);
            }
            else
            {
                return new PlayerTooPoorToBuildHouseEvent(player.Monopoly.Id, player.Id, Id, player.Money, UpgradePrice);
            }

        }
        else
        {
            return new PlayerCannotBuildHouseEvent(player.Monopoly.Id, player.Id, Id);
        }

    }
}

public class StartPoint : Block
{
    public StartPoint(string id) : base(id)
    {
    }
}

public class Jail : Block
{
    public Jail(string id) : base(id)
    {
    }
}

public class ParkingLot : Block
{
    public ParkingLot(string id) : base(id)
    {
    }
}

public class Station : Land
{
    public Station(string id, decimal price = 1000, string lot = "S") : base(id, price, lot)
    {
    }

    public override void Upgrade()
    {
        throw new Exception("車站不能蓋房子！");
    }

    public override decimal CalcullateToll(Player payee)
    {
        int lotCount = payee.LandContractList.Count(t => t.Land.Lot == lot);

        return _price * lotCount;
    }

    public override DomainEvent BuildHouse(Player player)
    {
        return new PlayerCannotBuildHouseEvent(player.Monopoly.Id, player.Id, Id);
    }
}