using Application.Common;
using Domain.Common;

namespace Application.Usecases;

public record PlayerPreparedRequest(string GameId, string PlayerId)
    : Request(GameId, PlayerId);

public class PlayerPreparedUsecase : Usecase<PlayerPreparedRequest>
{
    public PlayerPreparedUsecase(IRepository repository, IEventBus<DomainEvent> eventBus)
        : base(repository, eventBus)
    {
    }

    public override async Task ExecuteAsync(PlayerPreparedRequest request)
    {
        //查
        var game = Repository.FindGameById(request.GameId).ToDomain();

        //改
        game.PlayerPrepare(request.PlayerId);

        //存
        Repository.Save(game);

        //推
        await EventBus.PublishAsync(game.DomainEvents);
    }
}