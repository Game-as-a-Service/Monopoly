namespace Shared.Domain;

public abstract class IBlock
{
    public string Id { get; }
    public IBlock? Up { get; set; }
    public IBlock? Down { get; set; }
    public IBlock? Left { get; set; }
    public IBlock? Right { get; set; }

    public IBlock(string id)
    {
        Id = id;
    }

    public abstract IBlock? GetDirectionBlock(Map.Direction d);
}

public class Block: IBlock
{
    public Block(string id) : base(id)
    {
    }

    public override IBlock? GetDirectionBlock(Map.Direction d)
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