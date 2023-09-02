using Application.Common;
using Domain.Common;

namespace Application.Usecases;

public record RedeemRequest(string GameId, string PlayerId, string BlockId)
    : Request(GameId, PlayerId);

public class RedeemUsecase : Usecase<RedeemRequest>
{
    public RedeemUsecase(IRepository repository, IEventBus<DomainEvent> eventBus)
        : base(repository, eventBus)
    {
    }

    public override async Task ExecuteAsync(RedeemRequest request)
    {
        //查
        var game = Repository.FindGameById(request.GameId).ToDomain();

        //改
        game.RedeemLandContract(request.PlayerId, request.BlockId);

        //存
        Repository.Save(game);

        //推
        await EventBus.PublishAsync(game.DomainEvents);
    }
}