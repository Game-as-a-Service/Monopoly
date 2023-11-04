using Application.Common;
using Domain.Common;

namespace Application.Usecases;

public record GameStartRequest(string GameId, string PlayerId)
    : Request(GameId, PlayerId);

public class GameStartUsecase : Usecase<GameStartRequest>
{
    public GameStartUsecase(IRepository repository, IEventBus<DomainEvent> eventBus)
        : base(repository, eventBus)
    {
    }

    public override async Task ExecuteAsync(GameStartRequest request)
    {
        //查
        var game = Repository.FindGameById(request.GameId).ToDomain();

        //改
        game.GameStart(request.PlayerId);

        //存
        Repository.Save(game);

        //推
        await EventBus.PublishAsync(game.DomainEvents);
    }
}