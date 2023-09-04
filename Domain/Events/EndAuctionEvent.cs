using Domain.Common;

namespace Domain.Events;

public record EndAuctionEvent(string PlayerId, decimal PlayerMoney, string BlockId, string? Owner, decimal OwnerMoney)
    : DomainEvent;