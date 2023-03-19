using Domain.Common;

namespace Application.Common;

public interface IEventBus<TEvent> where TEvent : DomainEvent
{
    public Task PublishAsync(IEnumerable<TEvent> events);
}