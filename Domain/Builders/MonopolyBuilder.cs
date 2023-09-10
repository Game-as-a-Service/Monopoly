#pragma warning disable CS8618 // 退出建構函式時，不可為 Null 的欄位必須包含非 Null 值。請考慮宣告為可為 Null。
using Domain.Interfaces;
using System.Linq.Expressions;

namespace Domain.Builders;

public class MonopolyBuilder
{
    public string GameId { get; private set; }

    public List<Player> Players { get; private set; }

    public string HostId { get; private set; }

    public IDice[] Dices { get; private set; }

    public CurrentPlayerState CurrentPlayerState { get; private set; }

    public Map Map { get; private set; }
    public int Rounds { get; private set; }

    public MonopolyBuilder()
    {
        Players = new();
    }

    public MonopolyBuilder WithId(string id)
    {
        GameId = id;
        return this;
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

    public MonopolyBuilder WithPlayer(string id, Expression<Func<PlayerBuilder, PlayerBuilder>> expression)
    {
        var f = expression.Compile();
        var playerBuilder = new PlayerBuilder(id);
        f(playerBuilder);
        playerBuilder.WithMap(Map);
        Players.Add(playerBuilder.Build());
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
                            CurrentPlayerState,
                            Dices,
                            Rounds
                            );
    }

    public MonopolyBuilder WithRounds(int rounds)
    {
        Rounds = rounds;
        return this;
    }

    public MonopolyBuilder WithDices(IDice[] dices)
    {
        Dices = dices;
        return this;
    }

    
}
