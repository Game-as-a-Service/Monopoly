using Shared.Domain.Exceptions;
using static Shared.Domain.Map;

namespace Shared.Domain;

public class Chess
{
    private readonly Player player;
    private readonly Map map;
    private Block currentBlock;
    private Direction currentDirection;

    public Chess(Player player, Map map, Block currentBlock, Direction currentDirection)
    {
        this.player = player;
        this.map = map;
        this.currentBlock = currentBlock;
        this.currentDirection = currentDirection;
    }

    public Block CurrentBlock => currentBlock;
    public Direction CurrentDirection => currentDirection;

    /// <summary>
    /// 移動棋子
    /// 當前棋子會移動到下一個區塊
    /// 直到移動次數為0
    /// 或是可選方向多於一個
    /// </summary>
    /// <param name="moveCount"></param>
    public void Move(int moveCount)
    {
        for (int i = 0; i < moveCount; i++)
        {
            var nextBlock = CurrentBlock.GetDirectionBlock(CurrentDirection);
            if (nextBlock is null)
            {
                throw new Exception("找不到下一個區塊");
            }
            currentBlock = nextBlock;
            var directions = DirectionOptions();
            if (directions.Count > 1)
            {
                // 可選方向多於一個
                // 代表棋子會停在這個區塊
                throw new PlayerNeedToChooseDirectionException(player, currentBlock, directions);
            }
            // 只剩一個方向
            // 代表棋子會繼續往這個方向移動
            currentDirection = directions.First();
        }
    }

    internal void ChangeDirection(Direction direction)
    {
        if (direction == currentDirection.Opposite())
        {
            throw new Exception("不能選擇原本的方向");
        }
        if (!DirectionOptions().Contains(direction))
        {
            throw new Exception("不能選擇這個方向");
        }
        currentDirection = direction;
    }

    internal void SetBlock(string blockId, Direction direction)
    {
        currentBlock = map.FindBlockById(blockId);
        currentDirection = direction;
    }

    private List<Direction> DirectionOptions()
    {
        var directions = CurrentBlock.Directions;
        directions.Remove(CurrentDirection.Opposite());
        return directions;
    }
}