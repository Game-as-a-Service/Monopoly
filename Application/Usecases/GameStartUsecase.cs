using Application.Common;
using Domain.Common;

namespace Application.Usecases;

public record GameStartRequest(string GameId, string PlayerId)
    : Request(GameId, PlayerId);

public record GameStartResponse(IReadOnlyList<DomainEvent> Events) : CommandResponse(Events);

public class GameStartUsecase(IRepository repository)
    : Usecase<GameStartRequest, GameStartResponse>(repository)
{
    public override async Task ExecuteAsync(GameStartRequest request, IPresenter<GameStartResponse> presenter)
    {
        //查
        var game = Repository.FindGameById(request.GameId).ToDomain();

        //改
        game.GameStart(request.PlayerId);

        //存
        Repository.Save(game);

        //推
        await presenter.PresentAsync(new GameStartResponse(game.DomainEvents));
    }
}