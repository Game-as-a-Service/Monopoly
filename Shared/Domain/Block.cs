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
    public Land(string id) : base(id)
    {
    }
}