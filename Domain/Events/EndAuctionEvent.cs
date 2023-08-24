using Domain.Common;

namespace Domain.Events;

public record EndAuctionEvent(string GameId, string PlayerId, decimal PlayerMoney, string BlockId, string? Owner, decimal OwnerMoney)
    : DomainEvent(GameId);