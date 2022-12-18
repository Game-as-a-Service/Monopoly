namespace Shared.Domain;

public abstract class Block
{
    public string Id { get; }
    public Block? Up { get; set; }
    public Block? Down { get; set; }
    public Block? Left { get; set; }
    public Block? Right { get; set; }

    public Block(string id)
    {
        Id = id;
    }

    public abstract Block? GetDirectionBlock(Map.Direction d);
}

public class Land: Block
{
    public Land(string id) : base(id)
    {
    }

    public override Block? GetDirectionBlock(Map.Direction d)
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