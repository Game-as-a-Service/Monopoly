using static System.Net.Mime.MediaTypeNames;

namespace Domain.Builders;

public class PlayerBuilder
{
    public string Id { get; set; }
    public decimal Money { get; set; }
    public string BlockId { get; set; }
    public Direction CurrentDirection { get; set; }
    public List<(string LandId, bool InMortgage, int Deadline)> LandContracts { get; set; }
    public bool Bankrupt { get; set; }

    public PlayerBuilder(string id)
    {
        Id = id;
        Money = 15000;
        BlockId = "StartPoint";
        CurrentDirection = Direction.Right;
        LandContracts = new();
    }

    public PlayerBuilder WithMoney(decimal money)
    {
        Money = money;
        return this;
    }

    public PlayerBuilder WithPosition(string blockId, Direction direction)
    {
        BlockId = blockId;
        CurrentDirection = direction;
        return this;
    }

    public PlayerBuilder WithLandContract(string LandId, bool InMortgage = false, int Deadline = 10)
    {
        LandContracts.Add(new (LandId, InMortgage, Deadline));
        return this;
    }

    public PlayerBuilder WithBankrupt()
    {
        Bankrupt = true;
        return this;
    }

    public Player Build()
    {
        Player player = new(Id, Money);
        Chess chess = new(player, BlockId, Enum.Parse<Map.Direction>(CurrentDirection.ToString()));
        player.Chess = chess;
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
