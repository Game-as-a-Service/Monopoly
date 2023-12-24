using Application.Common;
using Domain.Common;

namespace Server.Hubs;

public class SignalrDefaultPresenter<TResponse>(IEventBus<DomainEvent> eventBus) : IPresenter<TResponse> 
    where TResponse : CommandResponse
{
    public async Task PresentAsync(TResponse response)
    {
        await eventBus.PublishAsync(response.Events);
    }
}