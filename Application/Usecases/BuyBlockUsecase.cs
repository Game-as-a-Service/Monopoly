using Application.Common;
using Domain.Common;

namespace Application.Usecases;

public record BuyBlockRequest(string GameId, string PlayerId, string LandID)
    : Request(GameId, PlayerId);

public class BuyBlockUsecase : Usecase<BuyBlockRequest>
{
    public BuyBlockUsecase(IRepository repository, IEventBus<DomainEvent> eventBus)
        : base(repository, eventBus)
    {
    }

    public override async Task ExecuteAsync(BuyBlockRequest request)
    {
        //查
        var game = Repository.FindGameById(request.GameId);

        //改
        game.PlayerRollDice(request.PlayerId);

        //存
        Repository.Save(game);

        //推
        await EventBus.PublishAsync(game.DomainEvents);
    }
}