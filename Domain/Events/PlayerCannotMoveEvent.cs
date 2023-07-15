using Domain.Common;

namespace Domain.Events;

public record PlayerCannotMoveEvent(string GameId, string PlayerId, int SuspendRounds) : DomainEvent(GameId);