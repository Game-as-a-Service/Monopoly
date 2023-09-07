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
    private bool isChooseDirection = true;

    public Chess(Player player,
                 string currentBlockId,
                 Direction currentDirection,
                 int remainingSteps,
                 bool isChooseDirection)
    {
        this.player = player;
        this.currentBlockId = currentBlockId;
        this.currentDirection = currentDirection;
        this.remainingSteps = remainingSteps;
        this.isChooseDirection = isChooseDirection;
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
        while (remainingSteps > 0)
        {
            var nextBlock = map.FindBlockById(currentBlockId).GetDirectionBlock(CurrentDirection) ?? throw new Exception("找不到下一個區塊");
            currentBlockId = nextBlock.Id;
            remainingSteps--;
            yield return new ChessMovedEvent(player.Id, currentBlockId, currentDirection.ToString(), remainingSteps);
            if (currentBlockId == "Start" && remainingSteps > 0) // 如果移動到起點，且還有剩餘步數，則獲得獎勵金
            {
                player.Money += 3000;
                yield return new ThroughStartEvent(player.Id, 3000, player.Money);
            }
            var directions = DirectionOptions(map);
            if (directions.Count > 1)
            {
                // 可選方向多於一個
                // 代表棋子會停在這個區塊
                isChooseDirection = false;
                yield return new PlayerNeedToChooseDirectionEvent(
                    player.Id,
                    directions.Select(d => d.ToString()).ToArray());
                yield break;
            }
            // 只剩一個方向
            // 代表棋子會繼續往這個方向移動
            currentDirection = directions.First();
        }
        map.FindBlockById(currentBlockId).DoBlockAction(player);
        yield return map.FindBlockById(currentBlockId).OnBlockEvent(player);
    }

    public IEnumerable<DomainEvent> Move(Map map, int steps)
    {
        remainingSteps = steps;
        return Move(map);
    }

    internal IEnumerable<DomainEvent> ChangeDirection(Map map, Direction direction)
    {
        List<DomainEvent> events = new() { };
        if (isChooseDirection)
        {
            events.Add(new PlayerAlreadyChooseDirectionEvent(player.Id));
            return events;
        }
        if (!DirectionOptions(map).Contains(direction))
        {
            events.Add(new PlayerChooseInvalidDirectionEvent(player.Id, direction.ToString()));
            return events;
        }
        isChooseDirection = false;
        currentDirection = direction;
        events.Add(new PlayerChooseDirectionEvent(player.Id, direction.ToString()));
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