using Application.Common;
using Domain.Common;

namespace Application.Usecases;

public record BidRequest(string GameId, string PlayerId, decimal BidPrice)
    : Request(GameId, PlayerId);

public class BidUsecase : Usecase<BidRequest>
{
    public BidUsecase(IRepository repository, IEventBus<DomainEvent> eventBus)
        : base(repository, eventBus)
    {
    }

    public override async Task ExecuteAsync(BidRequest request)
    {
        //查
        var game = Repository.FindGameById(request.GameId).ToDomain();

        //改
        game.PlayerBid(request.PlayerId, request.BidPrice);

        //存
        Repository.Save(game);

        //推
        await EventBus.PublishAsync(game.DomainEvents);
    }
}