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

    internal List<Map.Direction> Directions => new List<Map.Direction>()
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

    internal Block? GetDirectionBlock(Map.Direction d)
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

    internal virtual Player? GetOwner()
    {
        return null;
    }

    internal virtual void UpdateOwner(Player Owner)
    {
        throw new Exception("此地不可購買！");
    }

    internal abstract DomainEvent GetEvent(Player player);
    internal abstract void DoBlockAction(Player player);
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

    internal List<DomainEvent> PayToll(Player player)
    {
        // payer 應該付過路費給 payee
        // 計算過路費

        Player? owner = landContract.Owner;
        List<DomainEvent> events = new();

        if (player.EndRoundFlag)
        {
            //throw new Exception("玩家不需要支付過路費");
            events.Add(new PlayerDoesntNeedToPayTollEvent(player.Monopoly.Id, player.Id, player.Money));
        }
        else if (owner.SuspendRounds > 0)
        {
            //throw new Exception("不需要支付過路費：Owner is in the " + payee.Chess.CurrentBlock.Id);
            events.Add(new PlayerDoesntNeedToPayTollEvent(player.Monopoly.Id, player.Id, player.Money));
        }
        else
        {
            decimal amount = CalcullateToll(owner);

            if (player.Money > amount)
            {
                player.EndRoundFlag = true;
                player.PayToll(owner, amount);
                events.Add(new PlayerPayTollEvent(player.Monopoly.Id, player.Id, player.Money, owner.Id, owner.Money));
            }
            else
            {
                if(!player.LandContractList.Any(l => !l.Mortgage))
                {
                    // 破產
                    player.PayToll(owner, player.Money);
                    events.Add(new PlayerPayTollEvent(player.Monopoly.Id, player.Id, player.Money, owner.Id, owner.Money));

                    events.Add(player.UpdateState());
                }
                else
                {
                    //throw new Exception("錢包餘額不足！");
                    events.Add(new PlayerTooPoorToPayTollEvent(player.Monopoly.Id, player.Id, player.Money, amount));
                }
            }
        }
        return events;
    }

    internal virtual decimal CalcullateToll(Player owner)
    {
        int lotCount = owner.LandContractList.Count(t => t.Land.Lot == lot);

        return _price * RATE_OF_HOUSE[_house] * RATE_OF_LOT[lotCount];
    }

    internal decimal GetPrice(string use)
    {
        return use switch
        {
            "Mortgage" or "UnSold" => _price * (1 + _house) * (decimal)0.7,
            "Redeem" => _price * (1 + _house),
            "Auction" => _price * (1 + _house) * (decimal)0.5,
            _ => _price * (1 + _house),
        };
    }

    internal override Player? GetOwner() => landContract.Owner;

    internal override void UpdateOwner(Player? owner)
    {
        landContract.Owner = owner;
    }

    internal virtual DomainEvent BuildHouse(Player player)
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

    internal override DomainEvent GetEvent(Player player)
    {
        Player? owner = GetOwner();
        var land = this;
        if (owner is null)
        {
            return new PlayerCanBuyLandEvent(player.Monopoly.Id, player.Id, land.Id, land.Price);
        }
        else if (owner == player)
        {
            if(player.LandContractList.Any(l => l.Land.Id == Id && !l.Mortgage))
            {
                return new PlayerCanBuildHouseEvent(player.Monopoly.Id, player.Id, land.Id, land.House, land.UpgradePrice);
            }
        }
        else if (owner!.SuspendRounds <= 0)
        {
            return new PlayerNeedsToPayTollEvent(player.Monopoly.Id, player.Id, owner.Id, land.CalcullateToll(owner));
        }
        return DomainEvent.EmptyEvent;
    }

    internal override void DoBlockAction(Player player)
    {
        Player? owner = GetOwner();
        if (owner is null)
        {
            return;
        }
        if (player.LandContractList.Any(l => l.Land.Id == Id && !l.Mortgage))
        {
            player.EnableUpgrade = true;
        }
        else if (owner.SuspendRounds <= 0)
        {
            player.EndRoundFlag = false;
            
        }
    }
}

public class StartPoint : Block
{
    public StartPoint(string id) : base(id)
    {
    }

    internal override void DoBlockAction(Player player)
    {
    }

    internal override DomainEvent GetEvent(Player player)
    {
        return new OnStartEvent(player.Monopoly.Id, player.Id, 3000, player.Money);
    }
}
public class Jail : Block
{
    public Jail(string id) : base(id)
    {
    }

    internal override void DoBlockAction(Player player)
    {
        player.SuspendRound("Jail");
    }

    internal override DomainEvent GetEvent(Player player)
    {
        return new PlayerCannotMoveEvent(player.Monopoly.Id, player.Id, player.SuspendRounds);
    }
}
public class ParkingLot : Block
{
    public ParkingLot(string id) : base(id)
    {
    }

    internal override void DoBlockAction(Player player)
    {
        player.SuspendRound("ParkingLot");
    }

    internal override DomainEvent GetEvent(Player player)
    {
        return new PlayerCannotMoveEvent(player.Monopoly.Id, player.Id, player.SuspendRounds);
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

    internal override decimal CalcullateToll(Player owner)
    {
        int lotCount = owner.LandContractList.Count(t => t.Land.Lot == lot);

        return _price * lotCount;
    }

    internal override DomainEvent BuildHouse(Player player)
    {
        return new PlayerCannotBuildHouseEvent(player.Monopoly.Id, player.Id, Id);
    }

    internal override DomainEvent GetEvent(Player player)
    {
        Player? owner = GetOwner();
        var land = this;
        if (owner is null)
        {
            return new PlayerCanBuyLandEvent(player.Monopoly.Id, player.Id, land.Id, land.Price);
        }
        else if (owner!.SuspendRounds <= 0)
        {
            DoBlockAction(player);
            return new PlayerNeedsToPayTollEvent(player.Monopoly.Id, player.Id, owner.Id, land.CalcullateToll(owner));
        }
        return DomainEvent.EmptyEvent;
    }

    internal override void DoBlockAction(Player player)
    {
        Player? owner = GetOwner();
        if (owner is null)
        {
            return;
        }
        if (owner!.SuspendRounds <= 0)
        {
            player.EndRoundFlag = false;
        }
    }
}