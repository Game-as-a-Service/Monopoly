#pragma warning disable CS8618 // 退出建構函式時，不可為 Null 的欄位必須包含非 Null 值。請考慮宣告為可為 Null。
using Domain.Interfaces;
using System.Linq.Expressions;

namespace Domain.Builders;

public class MonopolyBuilder
{
    public string GameId { get; private set; }

    public List<PlayerBuilder> PlayerBuilders { get; private set; }

    public string HostId { get; private set; }

    public IDice[] Dices { get; private set; }

    public CurrentPlayerState CurrentPlayerState { get; private set; }

    public Map Map { get; private set; }
    public int Rounds { get; private set; }

    public MonopolyBuilder()
    {
        PlayerBuilders = new();
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

    public MonopolyBuilder WithPlayer(string id, Expression<Func<PlayerBuilder, PlayerBuilder>>? expression = null)
    {
        var playerBuilder = new PlayerBuilder(id);
        if (expression is not null)
        {
            var f = expression.Compile();
            f(playerBuilder);
        }
        
        PlayerBuilders.Add(playerBuilder);
        return this;
    }

    public MonopolyBuilder WithCurrentPlayer(string id, Expression<Func<CurrentPlayerStateBuilder, CurrentPlayerStateBuilder>>? expression = null)
    {
        var currentPlayerStateBuilder = new CurrentPlayerStateBuilder(id);
        if (expression is not null)
        {
            var f = expression.Compile();
            f(currentPlayerStateBuilder);
        }
        CurrentPlayerState = currentPlayerStateBuilder.Build();
        return this;
    }

    public MonopolyBuilder WithHost(string id)
    {
        HostId = id;
        return this;
    }

    public Monopoly Build()
    {
        var players = new List<Player>();
        PlayerBuilders.ForEach(builder =>
        {
            builder.WithMap(Map);
            players.Add(builder.Build());
        });
        return new Monopoly(GameId,
                            players.ToArray(),
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
