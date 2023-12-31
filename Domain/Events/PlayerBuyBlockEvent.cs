using Domain.Common;

namespace Domain.Events;

public record PlayerBuyBlockEvent(string PlayerId, string LandId)
    : DomainEvent;

public record PlayerBuyBlockMissedLandEvent(string PlayerId, string BlockId)
    : DomainEvent;

public record PlayerBuyBlockOccupiedByOtherPlayerEvent(string PlayerId, string LandId)
    : DomainEvent;

public record PlayerBuyBlockInsufficientFundsEvent(string PlayerId, string LandId, decimal Price)
    : DomainEvent;

public record HouseMaxEvent(string PlayerId, string LandId, int HouseCount)
    : DomainEvent;