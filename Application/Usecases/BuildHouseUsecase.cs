using Application.Common;
using Domain.Common;

namespace Application.Usecases;

public record BuildHouseRequest(string GameId, string PlayerId)
    : Request(GameId, PlayerId);

public class BuildHouseUsecase : Usecase<BuildHouseRequest>
{
    public BuildHouseUsecase(IRepository repository, IEventBus<DomainEvent> eventBus)
        : base(repository, eventBus)
    {
    }

    public override async Task ExecuteAsync(BuildHouseRequest request)
    {
        //查
        var game = Repository.FindGameById(request.GameId);

        //改
        game.BuildHouse(request.PlayerId);

        //存
        Repository.Save(game);

        //推
        await EventBus.PublishAsync(game.DomainEvents);
    }
}