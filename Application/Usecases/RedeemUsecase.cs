using Application.Common;
using Domain.Common;

namespace Application.Usecases;

public record RedeemRequest(string GameId, string PlayerId, string BlockId)
    : Request(GameId, PlayerId);

public record RedeemResponse(IReadOnlyList<DomainEvent> Events) : CommandResponse(Events);

public class RedeemUsecase(ICommandRepository repository, IEventBus<DomainEvent> eventBus)
    : CommandUsecase<RedeemRequest, RedeemResponse>(repository, eventBus)
{
    public override async Task ExecuteAsync(RedeemRequest request, IPresenter<RedeemResponse> presenter)
    {
        //查
        var game = Repository.FindGameById(request.GameId).ToDomain();

        //改
        game.RedeemLandContract(request.PlayerId, request.BlockId);

        //存
        Repository.Save(game);

        //推
        await presenter.PresentAsync(new RedeemResponse(game.DomainEvents));
    }
}