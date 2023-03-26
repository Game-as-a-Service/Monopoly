using Domain.Common;

namespace Domain.Events;

public record ChessMovedEvent(string GameId, string PlayerId, string BlockId, string Direction, int RemainingSteps): DomainEvent(GameId);
