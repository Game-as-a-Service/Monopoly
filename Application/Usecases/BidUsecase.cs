using Application.Common;
using Domain.Common;

namespace Application.Usecases;

public record BidRequest(string GameId, string PlayerId, decimal BidPrice)
    : Request(GameId, PlayerId);

public record BidResponse(IReadOnlyList<DomainEvent> Events) : CommandResponse(Events);

public class BidUsecase(IRepository repository)
    : Usecase<BidRequest, BidResponse>(repository)
{
    public override async Task ExecuteAsync(BidRequest request, IPresenter<BidResponse> presenter)
    {
        //查
        var game = Repository.FindGameById(request.GameId).ToDomain();

        //改
        game.PlayerBid(request.PlayerId, request.BidPrice);

        //存
        Repository.Save(game);

        //推
        await presenter.PresentAsync(new BidResponse(game.DomainEvents));
    }
}