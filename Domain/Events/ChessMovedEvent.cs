using Domain.Common;

namespace Domain.Events;

public record ChessMovedEvent(string PlayerId, string BlockId, string Direction, int RemainingSteps) : DomainEvent;