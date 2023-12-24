namespace Application.Common;
public abstract class QueryUsecase<TRequest, TResponse>(IRepository repository)
    where TRequest : Request
{
    protected IRepository Repository { get; } = repository;

    public abstract Task ExecuteAsync(TRequest request, IPresenter<TResponse> presenter);
}
