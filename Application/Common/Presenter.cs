namespace Application.Common;

public interface IPresenter<TResponse>
{
    public Task PresentAsync(TResponse response);
}