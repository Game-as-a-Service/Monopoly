using Domain.Common;

namespace Domain.Events;

public record PlayerMortgageEvent(string GameId, string PlayerId, decimal PlayerMoney, string BlockId, int DeadLine)
    : DomainEvent(GameId);

public record PlayerCannotMortgageEvent(string GameId, string PlayerId, decimal PlayerMoney, string BlockId)
    : DomainEvent(GameId);

public record MorgageDueEvent(string GameId, string PlayerId, string BlockId)
    : DomainEvent(GameId);

public record MorgageCountdownEvent(string GameId, string PlayerId, string BlockId, int DeadLine)
    : DomainEvent(GameId);