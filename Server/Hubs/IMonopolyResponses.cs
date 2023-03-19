using Domain.Common;

namespace Server.Hubs;

public interface IMonopolyResponses
{
    Task GameCreatedEvent(DomainEvent domainEvent);
}