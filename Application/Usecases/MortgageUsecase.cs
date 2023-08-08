using Application.Common;
using Domain.Common;

namespace Application.Usecases;

public record MortgageRequest(string GameId, string PlayerId, string BlockId)
    : Request(GameId, PlayerId);

public class MortgageUsecase : Usecase<MortgageRequest>
{
    public MortgageUsecase(IRepository repository, IEventBus<DomainEvent> eventBus)
        : base(repository, eventBus)
    {
    }

    public override async Task ExecuteAsync(MortgageRequest request)
    {
        //查
        var game = Repository.FindGameById(request.GameId);

        //改
        game.MortgageLandContract(request.PlayerId, request.BlockId);

        //存
        Repository.Save(game);

        //推
        await EventBus.PublishAsync(game.DomainEvents);
    }
}