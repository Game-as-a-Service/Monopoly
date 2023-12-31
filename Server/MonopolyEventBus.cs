using Application.Common;
using Domain.Common;
using Server.Common;

namespace Server;

internal class MonopolyEventBus : IEventBus<DomainEvent>
{
    private readonly Dictionary<Type, IMonopolyEventHandler> _handlers;

    public MonopolyEventBus(IEnumerable<IMonopolyEventHandler> handlers)
    {
        _handlers = handlers.ToDictionary(h => h.EventType, h => h);
    }
    
    public async Task PublishAsync(IEnumerable<DomainEvent> events)
    {
        foreach (var e in events)
        {
            var handler = GetHandler(e);
            await handler!.HandleAsync(e);
        }
    }

    private IMonopolyEventHandler GetHandler(DomainEvent e)
    {
        var type = e.GetType();
        if (!_handlers.TryGetValue(type, out var handler))
        {
            throw new InvalidOperationException($"Handler for {type} not registered");
        }
        return handler;
    }
}