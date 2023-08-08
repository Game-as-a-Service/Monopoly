using Domain.Common;

namespace Domain.Events;

public record PlayerMortgageEvent(string GameId, string PlayerId, decimal PlayerMoney, string BlockId, int DeadLine) 
    : DomainEvent(GameId);

    public record PlayerCannotMortgageEvent(string GameId, string PlayerId, decimal PlayerMoney, string BlockId) 
    : DomainEvent(GameId);