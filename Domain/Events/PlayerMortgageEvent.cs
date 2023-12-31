using Domain.Common;

namespace Domain.Events;

public record PlayerMortgageEvent(string PlayerId, decimal PlayerMoney, string BlockId, int DeadLine)
    : DomainEvent;

public record PlayerCannotMortgageEvent(string PlayerId, decimal PlayerMoney, string LandId)
    : DomainEvent;

public record MortgageDueEvent(string PlayerId, string LandId)
    : DomainEvent;

public record MortgageCountdownEvent(string PlayerId, string BlockId, int DeadLine)
    : DomainEvent;