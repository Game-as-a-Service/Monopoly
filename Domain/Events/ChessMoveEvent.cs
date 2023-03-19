using Domain.Common;

namespace Domain.Events;

public record ChessMoveEvent(string GameId, string PlayerId, string BlockId): DomainEvent(GameId);
