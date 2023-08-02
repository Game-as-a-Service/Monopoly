using Domain.Common;

namespace Domain.Events;

public record PlayerNeedsToPayTollEvent(string GameId, string PlayerId, string ownerId, decimal toll) 
    : DomainEvent(GameId);

public record PlayerPayTollEvent(string GameId, string PlayerId, decimal PlayerMoney, string ownerId, decimal ownerMoney) 
    : DomainEvent(GameId);

public record PlayerDoesntNeedToPayTollEvent(string GameId, string PlayerId, decimal PlayerMoney) 
    : DomainEvent(GameId);

public record PlayerTooPoorToPayTollEvent(string GameId, string PlayerId, decimal PlayerMoney, decimal toll) 
    : DomainEvent(GameId);