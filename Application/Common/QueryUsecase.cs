namespace Application.Common;
public abstract class QueryUsecase<TRequest, TResponse> where TRequest : Request
{
    protected IRepository Repository { get; }
    public QueryUsecase(IRepository repository)
    {
        Repository = repository;
    }
    public abstract Task ExecuteAsync(TRequest request, IPresenter<TResponse> presenter);
}
