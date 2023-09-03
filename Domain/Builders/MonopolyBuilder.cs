namespace Domain.Builders;

internal class MonopolyBuilder
{
    public string GameId { get; private set; }

    public List<Player> Players { get; private set; } = new();

    public string HostId { get; private set; }

    public int[] Dices { get; private set; }

    public CurrentPlayerState CurrentPlayerState { get; private set; }

    public Map Map { get; private set; }

    public MonopolyBuilder(string id)
    {
        GameId = id;
    }
    public MonopolyBuilder WithMap(Map map)
    {
        Map = map;
        return this;
    }

    public MonopolyBuilder WithPlayer(Player player)
    {
        Players.Add(player);
        return this;
    }

    public MonopolyBuilder WithCurrentPlayer(CurrentPlayerState currentPlayerState)
    {
        CurrentPlayerState = currentPlayerState;
        return this;
    }

    public MonopolyBuilder WithHost(string id)
    {
        HostId = id;
        return this;
    }

    public Monopoly Build()
    {
        return new Monopoly(GameId,
                            Players.ToArray(),
                            Map,
                            HostId,
                            CurrentPlayerState
                            );
    }
}
