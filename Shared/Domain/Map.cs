namespace Shared.Domain;

public class Map
{
    private readonly Block?[][] _blocks;
    public Map(Block?[][] blocks){
        _blocks = blocks;
        GererateBlockConnection(blocks);
    }
    private static void GererateBlockConnection(Block?[][] blocks){
        for (int i = 0; i < blocks.Length; i++)
        {
            for (int j = 0; j < blocks[i].Length; j++)
            {
                var block = blocks[i][j];
                if (block is null) continue;

                if (i > 0)
                {
                    block.Up = blocks[i - 1][j];
                }
                if (i < blocks.Length - 1)
                {
                    block.Down = blocks[i + 1][j];
                }
                if (j > 0)
                {
                    block.Left = blocks[i][j - 1];
                }
                if (j < blocks[i].Length - 1)
                {
                    block.Right = blocks[i][j + 1];
                }
            }
        }
    }

    public Block FindBlockById(string blockId)
    {
        foreach (var iBlock in from block in _blocks
                               from iBlock in block
                               where iBlock?.Id == blockId
                               select iBlock)
        {
            return iBlock;
        }
        throw new Exception("找不到該區塊");
    }

    public enum Direction
    {
        Up,
        Down,
        Left,
        Right,
        None
    }
}

public static class DirectionExtension
{
    public static Map.Direction Opposite(this Map.Direction direction)
    {
        return direction switch
        {
            Map.Direction.Up => Map.Direction.Down,
            Map.Direction.Down => Map.Direction.Up,
            Map.Direction.Left => Map.Direction.Right,
            Map.Direction.Right => Map.Direction.Left,
            _ => throw new ArgumentOutOfRangeException(nameof(direction), direction, null)
        };
    }
}