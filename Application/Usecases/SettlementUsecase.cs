using Application.Common;
using Domain.Common;

namespace Application.Usecases;

public record SettlementRequest(string GameId, string PlayerId)
    : Request(GameId, PlayerId);

public class SettlementUsecase : Usecase<SettlementRequest>
{
    public SettlementUsecase(IRepository repository, IEventBus<DomainEvent> eventBus)
        : base(repository, eventBus)
    {
    }

    public override async Task ExecuteAsync(SettlementRequest request)
    {
        //查
        var game = Repository.FindGameById(request.GameId).ToDomain();

        //改
        game.Settlement();

        //存
        Repository.Save(game);

        //推
        await EventBus.PublishAsync(game.DomainEvents);
    }
}