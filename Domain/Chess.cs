using Domain.Common;
using Domain.Events;
using static Domain.Map;

namespace Domain;

public class Chess
{
    private readonly Player player;
    private string currentBlockId;
    private Direction currentDirection;
    private int remainingSteps;

    public Chess(Player player, string currentBlockId, Direction currentDirection, int remainingSteps = 0)
    {
        this.player = player;
        this.currentBlockId = currentBlockId;
        this.currentDirection = currentDirection;
        this.remainingSteps = remainingSteps;
    }

    public Direction CurrentDirection => currentDirection;

    public int RemainingSteps => remainingSteps;

    public string CurrentBlockId => currentBlockId;

    /// <summary>
    /// 移動棋子
    /// 從 RemainingSteps 開始移動
    /// 直到移動次數為0
    /// </summary>
    private IEnumerable<DomainEvent> Move(Map map)
    {
        while (RemainingSteps > 0)
        {
            var nextBlock = map.FindBlockById(currentBlockId).GetDirectionBlock(CurrentDirection) ?? throw new Exception("找不到下一個區塊");
            currentBlockId = nextBlock.Id;
            remainingSteps--;
            if (currentBlockId == "Start" && remainingSteps > 0) // 如果移動到起點，且還有剩餘步數，則獲得獎勵金
            {
                player.Money += 3000;
                yield return new ThroughStartEvent(player.Monopoly.Id, player.Id, 3000, player.Money);
            }
            var directions = DirectionOptions(map);
            if (directions.Count > 1)
            {
                // 可選方向多於一個
                // 代表棋子會停在這個區塊
                yield return new ChessMovedEvent(player.Monopoly.Id, player.Id, currentBlockId, currentDirection.ToString(), remainingSteps);
                yield return new PlayerNeedToChooseDirectionEvent(
                    player.Monopoly.Id,
                    player.Id,
                    directions.Select(d => d.ToString()).ToArray());
                yield break;
            }
            // 只剩一個方向
            // 代表棋子會繼續往這個方向移動
            currentDirection = directions.First();
            yield return new ChessMovedEvent(player.Monopoly.Id, player.Id, currentBlockId, currentDirection.ToString(), remainingSteps);
        }
        map.FindBlockById(currentBlockId).DoBlockAction(player);
        yield return map.FindBlockById(currentBlockId).GetEvent(player);
    }

    internal List<DomainEvent> Move(Map map, int moveCount)
    {
        remainingSteps = moveCount;
        return Move(map).ToList();
    }

    internal List<DomainEvent> ChangeDirection(Map map, Direction direction)
    {
        if (direction == currentDirection.Opposite())
        {
            throw new Exception("不能選擇原本的方向");
        }
        if (!DirectionOptions(map).Contains(direction))
        {
            throw new Exception("不能選擇這個方向");
        }
        currentDirection = direction;
        List<DomainEvent> events = new() { new PlayerChooseDirectionEvent(player.Monopoly.Id, player.Id, direction.ToString()) };
        events.AddRange(Move(map));
        return events;
    }

    private List<Direction> DirectionOptions(Map map)
    {
        var directions = map.FindBlockById(currentBlockId).Directions;
        directions.Remove(CurrentDirection.Opposite());
        return directions;
    }
}