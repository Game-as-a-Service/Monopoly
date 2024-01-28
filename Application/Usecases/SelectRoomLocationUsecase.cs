using Application.Common;
using Domain.Common;

namespace Application.Usecases;

public record SelectLocationRequest(string GameId, string PlayerId, int LocationID)
    : Request(GameId, PlayerId);

public record SelectLocationResponse(IReadOnlyList<DomainEvent> Events) : CommandResponse(Events);

public class SelectLocationUsecase(ICommandRepository repository, IEventBus<DomainEvent> eventBus)
    : CommandUsecase<SelectLocationRequest, SelectLocationResponse>(repository, eventBus)
{
    public override async Task ExecuteAsync(SelectLocationRequest request, IPresenter<SelectLocationResponse> presenter)
    {
        //查
        var game = Repository.FindGameById(request.GameId).ToDomain();

        //改
        game.SelectLocation(request.PlayerId, request.LocationID);

        //存
        Repository.Save(game);

        //推
        await presenter.PresentAsync(new SelectLocationResponse(game.DomainEvents));
    }
}