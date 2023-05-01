using Domain.Common;

namespace Domain.Events;

public record PlayerPayTollEvent(string GameId, string PlayerId, string ownerId, decimal toll) : DomainEvent(GameId);