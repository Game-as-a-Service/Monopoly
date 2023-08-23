using Domain.Common;

namespace Domain.Events;

public record PlayerBidEvent(string GameId, string PlayerId, string BlockId, decimal HighestPrice)
    : DomainEvent(GameId);

public record PlayerBidFailEvent(string GameId, string PlayerId, string BlockId, decimal BidPrice, decimal HighestPrice)
    : DomainEvent(GameId);

public record PlayerTooPoorToBidEvent(string GameId, string PlayerId, decimal PlayerMoney, decimal BidPrice, decimal HighestPrice)
    : DomainEvent(GameId);