using Domain.Common;

namespace Server.Hubs.EventHandlers;

internal interface IMonopolyEventHandler<in TEvent> where TEvent : DomainEvent
{
    Task Handle(TEvent e);
}