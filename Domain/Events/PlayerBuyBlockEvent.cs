using Domain.Common;

namespace Domain.Events;

public record PlayerBuyBlockEvent(string GameId, string PlayerId, string BlockId, decimal landMoney)
    : DomainEvent(GameId);
