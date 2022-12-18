namespace Shared.Domain;

public class Map
{
    private readonly Block[][] _blocks;

    private readonly Dictionary<Player, (Block block, Direction direction)> _playerPositionDictionary = new();
    public Map(Block[][] blocks){
        _blocks = blocks;
        GererateBlockConnection(blocks);
    }
    private void GererateBlockConnection(Block[][] blocks){
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
    public void SetPlayerToBlock(Player player, string blockId, Direction direction)
    {
        _playerPositionDictionary[player] = (FindBlockById(blockId), direction); 
    }

    private Block FindBlockById(string blockId)
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

    public void PlayerMove(Player player, int moveCount)
    {
        var (block, direction) = _playerPositionDictionary[player];
        for (int i = 0; i < moveCount; i++)
        {
            var (nextBlock, nextBlockDirections) = GetNextBlockAndDirections(block, direction);
            block = nextBlock;
            if (nextBlockDirections.Length == 1)
            {
                direction = nextBlockDirections[0];
            }
            else
            {
                throw new Exception("需要選擇方向");
            }
        }
        _playerPositionDictionary[player] = (block, direction);
    }

    // 得到下一個 Block 及方向
    private (Block NextBlock, Direction[] NextBlockDirections) GetNextBlockAndDirections(Block currentBlock, Direction currentDirection)
    {
        // 先得到 下一個 Block
        var nextBlock = currentDirection switch
        {
            Direction.Up => currentBlock.Up!,
            Direction.Down => currentBlock.Down!,
            Direction.Left => currentBlock.Left!,
            Direction.Right => currentBlock.Right!,
            _ => throw new ArgumentOutOfRangeException(nameof(currentDirection), currentDirection, null)
        };
        // 再得到 下一個 Block 可以移動的方向
        var nextBlockDirections = new[]
        {
            (direction: Direction.Up, block: nextBlock.Up),
            (direction: Direction.Down, block: nextBlock.Down),
            (direction: Direction.Left, block: nextBlock.Left),
            (direction: Direction.Right, block: nextBlock.Right)
        }
        .Where(x => x.block != null && x.direction != currentDirection.Opposite())
        .Select(x => x.direction)
        .ToArray();
        if (nextBlockDirections.Length == 0)
        {
            throw new Exception("無法移動");
        }
        return (nextBlock, nextBlockDirections);
    }

    public (Block block, Direction direction) GetPlayerPositionAndDirection(Player player) => _playerPositionDictionary[player];

    public enum Direction
    {
        Up,
        Down,
        Left,
        Right
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