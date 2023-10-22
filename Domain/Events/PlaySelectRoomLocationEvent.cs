using Domain.Common;

namespace Domain.Events;

public record PlaySelectRoomLocationEvent(string PlayerId, int LocationId)
    : DomainEvent;
