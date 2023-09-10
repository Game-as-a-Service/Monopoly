using static System.Net.Mime.MediaTypeNames;

namespace Domain.Builders;

public class PlayerBuilder
{
    public string Id { get; set; }
    public decimal Money { get; set; }
    public string BlockId { get; set; }
    public Map.Direction CurrentDirection { get; set; }
    public Map Map { get; set; }
    public List<(string LandId, bool InMortgage, int Deadline)> LandContracts { get; set; }
    public bool Bankrupt { get; set; }
    public int BankruptRounds { get; private set; }
    public int RemainingSteps { get; set; }
    public bool IsChooseDirection { get; set; }

    public PlayerBuilder(string id)
    {
        Id = id;
        Money = 15000;
        BlockId = "Start";
        CurrentDirection = Map.Direction.Right;
        LandContracts = new();
        Bankrupt = false;
        RemainingSteps = 0;
        IsChooseDirection = true;
    }

    public PlayerBuilder WithMoney(decimal money)
    {
        Money = money;
        return this;
    }

    public PlayerBuilder WithPosition(string blockId, string direction)
    {
        BlockId = blockId;
        CurrentDirection = Enum.Parse<Map.Direction>(direction);
        return this;
    }

    public PlayerBuilder WithLandContract(string LandId, bool InMortgage = false, int Deadline = 10)
    {
        LandContracts.Add(new (LandId, InMortgage, Deadline));
        return this;
    }

    public PlayerBuilder WithBankrupt(int Rounds)
    {
        Bankrupt = true;
        BankruptRounds = Rounds;
        return this;
    }

    public PlayerBuilder WithMap(Map map)
    {
        Map = map;
        return this;
    }

    public PlayerBuilder WithRemainingSteps(int remainingSteps)
    {
        RemainingSteps = remainingSteps;
        return this;
    }

    public PlayerBuilder WithNotChooseDirection()
    {
        IsChooseDirection = false;
        return this;
    }

    public Player Build()
    {
        Player player = new(Id, Money, BankruptRounds);
        Chess chess = new(player: player,
                          currentBlockId: BlockId,
                          currentDirection: CurrentDirection,
                          remainingSteps: RemainingSteps,
                          isChooseDirection: IsChooseDirection);
        player.Chess = chess;
        player.SuspendRound(BlockId);
        if (LandContracts.Count > 0)
        {
            if (Map == null)
            {
                throw new InvalidOperationException("Map must be set!");
            }
        }
        foreach(var (LandId, InMortgage, Deadline) in LandContracts)
        {
            player.AddLandContract(new(player, Map.FindBlockById<Land>(LandId)));
            if (InMortgage)
            {
                player.LandContractList[^1].GetMortgage();

                #region 測試用
                player.LandContractList[^1].SetDeadLine(Deadline);
                #endregion
            }
        }
        return player;
    }

    public enum Direction
    {
        Up,
        Down,
        Left,
        Right
    }
}
