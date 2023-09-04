using Domain.Common;

namespace Domain.Events;

public record PlayerBuyBlockEvent(string PlayerId, string BlockId)
    : DomainEvent;

public record PlayerBuyBlockMissedLandEvent(string PlayerId, string BlockId)
    : DomainEvent;

public record PlayerBuyBlockOccupiedByOtherPlayerEvent(string PlayerId, string BlockId)
    : DomainEvent;

public record PlayerBuyBlockInsufficientFundsEvent(string PlayerId, string BlockId, decimal landMoney)
    : DomainEvent;

public record HouseMaxEvent(string PlayerId, string BlockId, int House)
    : DomainEvent;