using Domain.Common;

namespace Domain.Events;
public record PlayerSelectRoleEvent(string PlayerId, string RoleId) : DomainEvent;