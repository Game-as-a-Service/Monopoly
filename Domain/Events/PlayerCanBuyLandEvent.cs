using Domain.Common;

namespace Domain.Events;

public record PlayerCanBuyLandEvent(string PlayerId, string LandId, decimal Price)
    : DomainEvent;