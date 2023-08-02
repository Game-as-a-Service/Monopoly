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
    private List<DomainEvent> Move()
    {
        List<DomainEvent> events = new();
        while (RemainingSteps > 0)
        {
            var nextBlock = CurrentBlock.GetDirectionBlock(CurrentDirection) ?? throw new Exception("找不到下一個區塊");
            currentBlock = nextBlock;
            remainingSteps--;
            if (currentBlock is StartPoint && remainingSteps > 0) // 如果移動到起點，且還有剩餘步數，則獲得獎勵金
            {
                player.Money += 3000;
                events.Add(new ThroughStartEvent(player.Monopoly.Id, player.Id, 3000, player.Money));
            }
            var directions = DirectionOptions();
            if (directions.Count > 1)
            {
                // 可選方向多於一個
                // 代表棋子會停在這個區塊
                events.Add(new ChessMovedEvent(player.Monopoly.Id, player.Id, currentBlock.Id, currentDirection.ToString(), remainingSteps));
                events.Add(new PlayerNeedToChooseDirectionEvent(
                    player.Monopoly.Id,
                    player.Id,
                    directions.Select(d => d.ToString()).ToArray()));
                return events;
                //throw new PlayerNeedToChooseDirectionException(player, currentBlock, directions);
            }
            // 只剩一個方向
            // 代表棋子會繼續往這個方向移動
            currentDirection = directions.First();
            events.Add(new ChessMovedEvent(player.Monopoly.Id, player.Id, currentBlock.Id, currentDirection.ToString(), remainingSteps));
        }

        return events;
    }

    public List<DomainEvent> Move(int moveCount)
    {
        remainingSteps = moveCount;
        return Move();
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
        events.AddRange(GetLandEvent());
        return events;
    }

    public IEnumerable<DomainEvent> GetLandEvent()
    {
        if (RemainingSteps != 0) yield break;

        //TODO 感覺要套策略
        if (CurrentBlock is Land land)
        {
            Player? owner = land.GetOwner();
            if (owner is null)
            {
                yield return new PlayerCanBuyLandEvent(player.Monopoly.Id, player.Id, land.Id, land.Price);
            }
            else if (owner == player)
            {
                yield return new PlayerCanBuildHouseEvent(player.Monopoly.Id, player.Id, land.Id, land.House, land.UpgradePrice);
            }
            else if (owner!.Chess.CurrentBlock.Id != "Jail" && owner.Chess.CurrentBlock.Id != "ParkingLot")
            {
                yield return new PlayerNeedsToPayTollEvent(player.Monopoly.Id, player.Id, owner.Id, land.CalcullateToll(owner));
                player.EndRoundFlag = false;
            }
        }
        else if (CurrentBlock is StartPoint) // 如果移動到起點， 無法獲得獎勵金
        {
            yield return new OnStartEvent(player.Monopoly.Id, player.Id, 3000, player.Money);
        }
        else if (CurrentBlock is Jail) // 如果移動到監獄
        {
            yield return new PlayerCannotMoveEvent(player.Monopoly.Id, player.Id, 2);
        }
        else if (CurrentBlock is ParkingLot) // 如果移動到停車場
        {
            yield return new PlayerCannotMoveEvent(player.Monopoly.Id, player.Id, 1);
        }
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