using Domain.Common;

namespace Application.Common;

public abstract class Usecase<TRequest, TResponse>(IRepository repository)
    where TRequest : Request where TResponse : Response
{
    protected IRepository Repository { get; } = repository;

    public abstract Task ExecuteAsync(TRequest request, IPresenter<TResponse> presenter);
}