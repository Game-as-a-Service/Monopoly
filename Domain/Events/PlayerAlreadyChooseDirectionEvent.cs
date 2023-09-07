using Domain.Common;

namespace Domain.Events;

public record PlayerAlreadyChooseDirectionEvent(string PlayerId) : DomainEvent;
