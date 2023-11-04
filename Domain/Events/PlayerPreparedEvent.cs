using Domain.Common;

namespace Domain.Events;

public record PlayerPrepareEvent(string PlayerId, string PlayerState)
    : DomainEvent;

public record PlayerCannotPrepareEvent(string PlayerId, string PlayerState, string? RoleId, int LocationId)
    : DomainEvent;