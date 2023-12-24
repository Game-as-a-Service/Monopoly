using Application.Common;
using Domain.Common;

namespace Application.Usecases;

public record PlayerBuyLandRequest(string GameId, string PlayerId, string LandID)
    : Request(GameId, PlayerId);

public record PlayerBuyLandResponse(IReadOnlyList<DomainEvent> Events) : CommandResponse(Events);

public class PlayerBuyLandUsecase(IRepository repository)
    : Usecase<PlayerBuyLandRequest, PlayerBuyLandResponse>(repository)
{
    public override async Task ExecuteAsync(PlayerBuyLandRequest request, IPresenter<PlayerBuyLandResponse> presenter)
    {
        //查
        var game = Repository.FindGameById(request.GameId).ToDomain();

        //改
        game.BuyLand(request.PlayerId, request.LandID);

        //存
        Repository.Save(game);

        //推
        await presenter.PresentAsync(new PlayerBuyLandResponse(game.DomainEvents));
    }
}