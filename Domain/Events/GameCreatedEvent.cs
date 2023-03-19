using Domain.Common;

namespace Domain.Events;

public record GameCreatedEvent(string GameId) : DomainEvent(GameId);