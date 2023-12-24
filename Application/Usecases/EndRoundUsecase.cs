using Application.Common;
using Domain.Common;

namespace Application.Usecases;

public record EndRoundRequest(string GameId, string PlayerId)
    : Request(GameId, PlayerId);

public record EndRoundResponse(IReadOnlyList<DomainEvent> Events) : CommandResponse(Events);

public class EndRoundUsecase(IRepository repository)
    : Usecase<EndRoundRequest, EndRoundResponse>(repository)
{
    public override async Task ExecuteAsync(EndRoundRequest request, IPresenter<EndRoundResponse> presenter)
    {
        //查
        var game = Repository.FindGameById(request.GameId).ToDomain();

        //改
        game.EndRound();

        //存
        Repository.Save(game);

        //推
        await presenter.PresentAsync(new EndRoundResponse(game.DomainEvents));
    }
}