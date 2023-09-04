using Domain.Common;

namespace Domain.Events;

public record PlayerCanBuyLandEvent(string PlayerId, string BlockId, decimal landMoney)
    : DomainEvent;