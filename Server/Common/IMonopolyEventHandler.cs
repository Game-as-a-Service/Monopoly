using Domain.Common;

namespace Server.Common;

internal interface IMonopolyEventHandler
{
    public Type EventType { get; }
    Task HandleAsync(DomainEvent e);
}