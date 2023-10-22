using Domain.Common;

namespace Domain.Events;

public record PlayerHadSelectedDirectionEvent(string PlayerId) : DomainEvent;
