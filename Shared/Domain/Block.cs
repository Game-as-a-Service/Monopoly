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
}

public class Block: IBlock
{
    public Block(string id) : base(id)
    {
    }
}