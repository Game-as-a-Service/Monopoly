using Domain.Common;

namespace Domain.Events;

public record PlayerNeedsToPayTollEvent(string PlayerId, string ownerId, decimal toll) : DomainEvent;

public record PlayerPayTollEvent(string PlayerId, decimal PlayerMoney, string ownerId, decimal ownerMoney) : DomainEvent;

public record PlayerDoesntNeedToPayTollEvent(string PlayerId, decimal PlayerMoney) : DomainEvent;

public record PlayerTooPoorToPayTollEvent(string PlayerId, decimal PlayerMoney, decimal toll) : DomainEvent;