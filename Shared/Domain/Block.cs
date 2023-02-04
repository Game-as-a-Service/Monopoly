namespace Shared.Domain;

public abstract class Block
{
    public string Id { get; }
    public Block? Up { get; set; }
    public Block? Down { get; set; }
    public Block? Left { get; set; }
    public Block? Right { get; set; }
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
}

public class Land: Block
{
    private readonly decimal _price;
    private int house;
    public decimal Price => _price; // 土地購買價格
    // public int UpgradePrice => _price; // 升級價格
    // public int TollFee => _price; // 過路費
    public int House => house;
    public Land(string id, decimal price = 1000) : base(id)
    {
        _price = price;
    }

    public void Upgrade()
    {
        house++;
    }
}

public class StartPoint : Block
{
    public StartPoint(string id) : base(id)
    {
    }
}