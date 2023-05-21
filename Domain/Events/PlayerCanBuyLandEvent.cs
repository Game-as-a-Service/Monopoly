using Domain.Common;

namespace Domain.Events;

public record PlayerCanBuyLandEvent(string GameId, string PlayerId, string BlockId, decimal landMoney)
    : DomainEvent(GameId);
