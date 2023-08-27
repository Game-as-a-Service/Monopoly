using Domain.Common;
using Domain.Events;
using static Domain.Map;

namespace Domain;

public class Chess
{
    private readonly Player player;
    private readonly Map map;
    private Block currentBlock;
    private Direction currentDirection;
    private int remainingSteps;

    public Chess(Player player, Map map, Block currentBlock, Direction currentDirection, int remainingSteps = 0)
    {
        this.player = player;
        this.map = map;
        this.currentBlock = currentBlock;
        this.currentDirection = currentDirection;
        this.remainingSteps = remainingSteps;
    }

    public Block CurrentBlock => currentBlock;
    public Direction CurrentDirection => currentDirection;

    public int RemainingSteps => remainingSteps;

    /// <summary>
    /// 移動棋子
    /// 從 RemainingSteps 開始移動
    /// 直到移動次數為0
    /// </summary>
    private IEnumerable<DomainEvent> Move()
    {
        while (RemainingSteps > 0)
        {
            var nextBlock = CurrentBlock.GetDirectionBlock(CurrentDirection) ?? throw new Exception("找不到下一個區塊");
            currentBlock = nextBlock;
            remainingSteps--;
            if (currentBlock is StartPoint && remainingSteps > 0) // 如果移動到起點，且還有剩餘步數，則獲得獎勵金
            {
                player.Money += 3000;
                yield return new ThroughStartEvent(player.Monopoly.Id, player.Id, 3000, player.Money);
            }
            var directions = DirectionOptions();
            if (directions.Count > 1)
            {
                // 可選方向多於一個
                // 代表棋子會停在這個區塊
                yield return new ChessMovedEvent(player.Monopoly.Id, player.Id, currentBlock.Id, currentDirection.ToString(), remainingSteps);
                yield return new PlayerNeedToChooseDirectionEvent(
                    player.Monopoly.Id,
                    player.Id,
                    directions.Select(d => d.ToString()).ToArray());
                yield break;
                //throw new PlayerNeedToChooseDirectionException(player, currentBlock, directions);
            }
            // 只剩一個方向
            // 代表棋子會繼續往這個方向移動
            currentDirection = directions.First();
            yield return new ChessMovedEvent(player.Monopoly.Id, player.Id, currentBlock.Id, currentDirection.ToString(), remainingSteps);
        }
        currentBlock.DoBlockAction(player);
        yield return currentBlock.GetEvent(player);
    }

    public List<DomainEvent> Move(int moveCount)
    {
        remainingSteps = moveCount;
        return Move().ToList();
    }

    public List<DomainEvent> ChangeDirection(Direction direction)
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
        List<DomainEvent> events = new() { new PlayerChooseDirectionEvent(player.Monopoly.Id, player.Id, direction.ToString()) };
        events.AddRange(Move());
        return events;
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