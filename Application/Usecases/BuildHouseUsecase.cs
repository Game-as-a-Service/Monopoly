using Application.Common;
using Domain.Common;

namespace Application.Usecases;

public record BuildHouseRequest(string GameId, string PlayerId)
    : Request(GameId, PlayerId);

public record BuildHouseResponse(IReadOnlyList<DomainEvent> Events) : CommandResponse(Events);

public class BuildHouseUsecase(ICommandRepository repository, IEventBus<DomainEvent> eventBus)
    : CommandUsecase<BuildHouseRequest, BuildHouseResponse>(repository, eventBus)
{
    public override async Task ExecuteAsync(BuildHouseRequest request, IPresenter<BuildHouseResponse> presenter)
    {
        //查
        var game = Repository.FindGameById(request.GameId).ToDomain();

        //改
        game.BuildHouse(request.PlayerId);

        //存
        Repository.Save(game);

        //推
        await presenter.PresentAsync(new BuildHouseResponse(game.DomainEvents));
    }
}