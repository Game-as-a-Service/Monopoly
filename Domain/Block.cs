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

    private LandContract landContract;
    private readonly decimal _price;
    private int house;

    public decimal Price => _price; // 土地購買價格

    // public int UpgradePrice => _price; // 升級價格
    // public int TollFee => _price; // 過路費
    public int House => house;

    public Land(string id, decimal price = 1000, string lot = " ") : base(id)
    {
        _price = price;
        landContract = new LandContract(null, this);
        this.lot = lot;
    }

    public void Upgrade()
    {
        house++;
    }

    public void PayToll(Player payer)
    {
        // payer 應該付過路費給 payee
        // 計算過路費

        Player? payee = landContract.Owner;

        if (payer.EndRoundFlag)
        {
            throw new Exception("玩家不需要支付過路費");
        }
        else if (payee!.Chess.CurrentBlock.Id == "Jail" || payee.Chess.CurrentBlock.Id == "ParkingLot")
        {
            throw new Exception("不需要支付過路費：Owner is in the " + payee.Chess.CurrentBlock.Id);
        }
        else
        {
            int lotCount = payee.LandContractList.Count(t => t.Land.Lot == lot);

            decimal amount = _price * RATE_OF_HOUSE[house] * RATE_OF_LOT[lotCount];

            if (payer.Money > amount)
            {
                payer.EndRoundFlag = true;
                payer.PayToll(payee, amount);
            }
            else
            {
                throw new Exception("錢包餘額不足！");
            }
        }
    }

    public override Player? GetOwner()
    {
        return landContract.Owner;
    }

    public override void UpdateOwner(Player Owner)
    {
        landContract.Owner = Owner;
    }
}

public class StartPoint : Block
{
    public StartPoint(string id) : base(id)
    {
    }
}