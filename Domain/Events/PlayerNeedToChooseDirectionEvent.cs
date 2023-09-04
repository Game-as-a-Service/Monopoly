using Domain.Common;

namespace Domain.Events;

public record PlayerNeedToChooseDirectionEvent(string PlayerId, string[] Directions) : DomainEvent;