using Domain.Common;

namespace Domain.Events;

public record PlayerBuyBlockEvent(string GameId, string PlayerId, string BlockId)
    : DomainEvent(GameId);

public record PlayerBuyBlockMissedLandEvent(string GameId, string PlayerId, string BlockId)
    : DomainEvent(GameId);

public record PlayerBuyBlockOccupiedByOtherPlayerEvent(string GameId, string PlayerId, string BlockId)
    : DomainEvent(GameId);

public record PlayerBuyBlockInsufficientFundsEvent(string GameId, string PlayerId, string BlockId, decimal landMoney)
    : DomainEvent(GameId);

public record HouseMaxEvent(string GameId, string PlayerId, string BlockId, int House)
    : DomainEvent(GameId);