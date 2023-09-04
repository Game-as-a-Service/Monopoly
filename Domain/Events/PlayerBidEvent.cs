using Domain.Common;

namespace Domain.Events;

public record PlayerBidEvent(string PlayerId, string BlockId, decimal HighestPrice) : DomainEvent;

public record PlayerBidFailEvent(string PlayerId, string BlockId, decimal BidPrice, decimal HighestPrice) : DomainEvent;

public record PlayerTooPoorToBidEvent(string PlayerId, decimal PlayerMoney, decimal BidPrice, decimal HighestPrice) : DomainEvent;

public record CurrentPlayerCannotBidEvent(string PlayerId) : DomainEvent;