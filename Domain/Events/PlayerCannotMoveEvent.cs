using Domain.Common;

namespace Domain.Events;

public record PlayerCannotMoveEvent(string PlayerId, int SuspendRounds) : DomainEvent;