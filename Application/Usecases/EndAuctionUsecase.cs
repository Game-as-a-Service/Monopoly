using Application.Common;
using Domain.Common;

namespace Application.Usecases;

public record EndAuctionRequest(string GameId, string PlayerId)
    : Request(GameId, PlayerId);

public class EndAuctionUsecase : Usecase<EndAuctionRequest>
{
    public EndAuctionUsecase(IRepository repository, IEventBus<DomainEvent> eventBus)
        : base(repository, eventBus)
    {
    }

    public override async Task ExecuteAsync(EndAuctionRequest request)
    {
        //查
        var game = Repository.FindGameById(request.GameId);

        //改
        game.EndAuction();

        //存
        Repository.Save(game);

        //推
        await EventBus.PublishAsync(game.DomainEvents);
    }
}