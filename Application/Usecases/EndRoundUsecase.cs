using Application.Common;
using Domain.Common;

namespace Application.Usecases;

public record EndRoundRequest(string GameId, string PlayerId)
    : Request(GameId, PlayerId);

public class EndRoundUsecase : Usecase<EndRoundRequest>
{
    public EndRoundUsecase(IRepository repository, IEventBus<DomainEvent> eventBus)
        : base(repository, eventBus)
    {
    }

    public override async Task ExecuteAsync(EndRoundRequest request)
    {
        //查
        var game = Repository.FindGameById(request.GameId).ToDomain();

        //改
        game.EndRound();

        //存
        Repository.Save(game);

        //推
        await EventBus.PublishAsync(game.DomainEvents);
    }
}