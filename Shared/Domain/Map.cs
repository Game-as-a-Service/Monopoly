namespace Shared.Domain;

public class Map
{
    private readonly IBlock[][] _blocks;

    private readonly Dictionary<Player, (IBlock block, Direction direction)> _playerPositionDictionary = new();
    public Map(IBlock[][] blocks){
        _blocks = blocks;
        GererateBlockConnection(blocks);
    }
    private void GererateBlockConnection(IBlock[][] blocks){
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

    private IBlock FindBlockById(string blockId)
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
            block = GetNextBlock(block!, direction);
            var nextDirection = GetNextDirection(block, direction);
            if (nextDirection.Count() == 1)
            {
                direction = nextDirection.First();
            }
            else
            {
                throw new Exception("需要選擇方向");
            }
        }
        _playerPositionDictionary[player] = (block, direction);
    }

    private IBlock? GetNextBlock(IBlock block, Direction direction)
    {
        return direction switch
        {
            Direction.Up => block.Up,
            Direction.Down => block.Down,
            Direction.Left => block.Left,
            Direction.Right => block.Right,
            _ => throw new ArgumentOutOfRangeException(nameof(direction), direction, null)
        };
    }
    // 得到 block 可以移動的方向，且不能走回頭路
    private Direction[] GetNextDirection(IBlock block, Direction currentDirection)
    {
        var directions = new List<Direction>();
        if (block.Up != null && currentDirection != Direction.Down)
        {
            directions.Add(Direction.Up);
        }
        if (block.Down != null && currentDirection != Direction.Up)
        {
            directions.Add(Direction.Down);
        }
        if (block.Left != null && currentDirection != Direction.Right)
        {
            directions.Add(Direction.Left);
        }
        if (block.Right != null && currentDirection != Direction.Left)
        {
            directions.Add(Direction.Right);
        }
        return directions.ToArray();
    }

    public (IBlock block, Direction direction) GetPlayerPosition(Player player) => _playerPositionDictionary[player];

    public enum Direction
    {
        Up,
        Down,
        Left,
        Right
    }
}