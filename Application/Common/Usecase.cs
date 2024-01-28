using Domain.Common;

namespace Application.Common;

public abstract class Usecase<TRequest, TResponse>()
    where TRequest : Request where TResponse : Response
{
    public abstract Task ExecuteAsync(TRequest request, IPresenter<TResponse> presenter);
}

public abstract class CommandUsecase<TRequest, TResponse>(ICommandRepository repository, IEventBus<DomainEvent> eventBus)
    : Usecase<TRequest, TResponse>()
    where TRequest : Request where TResponse : Response
{
    protected ICommandRepository Repository { get; } = repository;
    protected IEventBus<DomainEvent> EventBus { get; } = eventBus;
}

public abstract class QueryUsecase<TRequest, TResponse>(ICommandRepository repository)
    where TRequest : Request
{
    protected ICommandRepository Repository { get; } = repository;

    public abstract Task ExecuteAsync(TRequest request, IPresenter<TResponse> presenter);
}
