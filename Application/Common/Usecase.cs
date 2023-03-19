using Domain.Common;

namespace Application.Common;

public abstract class Usecase<TRequest> where TRequest : Request
{
    protected IRepository Repository { get; }
    protected IEventBus<DomainEvent> EventBus { get; }

    public Usecase(IRepository repository, IEventBus<DomainEvent> eventBus)
    {
        Repository = repository;
        EventBus = eventBus;
    }

    public abstract Task ExecuteAsync(TRequest request);
}