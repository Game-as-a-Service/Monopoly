using Domain.Common;

namespace Domain.Events;

public record EndAuctionEvent(string PlayerId, decimal PlayerMoney, string LandId, string? OwnerId, decimal OwnerMoney)
    : DomainEvent;