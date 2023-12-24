using Application.Common;
using Domain.Common;

namespace Application.Usecases;

public record EndAuctionRequest(string GameId, string PlayerId)
    : Request(GameId, PlayerId);

public record EndAuctionResponse(IReadOnlyList<DomainEvent> Events) : CommandResponse(Events);

public class EndAuctionUsecase(IRepository repository)
    : Usecase<EndAuctionRequest, EndAuctionResponse>(repository)
{
    public override async Task ExecuteAsync(EndAuctionRequest request, IPresenter<EndAuctionResponse> presenter)
    {
        //查
        var game = Repository.FindGameById(request.GameId).ToDomain();

        //改
        game.EndAuction();

        //存
        Repository.Save(game);

        //推
        await presenter.PresentAsync(new EndAuctionResponse(game.DomainEvents));
    }
}