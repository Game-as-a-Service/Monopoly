using Application.Common;
using Domain.Common;

namespace Application.Usecases;

public record PlayerPreparedRequest(string GameId, string PlayerId)
    : Request(GameId, PlayerId);

public record PlayerPreparedResponse(IReadOnlyList<DomainEvent> Events) : CommandResponse(Events);

public class PlayerPreparedUsecase(IRepository repository)
    : Usecase<PlayerPreparedRequest, PlayerPreparedResponse>(repository)
{
    public override async Task ExecuteAsync(PlayerPreparedRequest request, IPresenter<PlayerPreparedResponse> presenter)
    {
        //查
        var game = Repository.FindGameById(request.GameId).ToDomain();

        //改
        game.PlayerPrepare(request.PlayerId);

        //存
        Repository.Save(game);

        //推
        await presenter.PresentAsync(new PlayerPreparedResponse(game.DomainEvents));
    }
}