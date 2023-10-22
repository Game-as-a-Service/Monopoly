using Application.Common;
using Domain.Common;

namespace Application.Usecases;

public record SelectLocationRequest(string GameId, string PlayerId, int LocationID)
    : Request(GameId, PlayerId);

public class SelectLocationUsecase : Usecase<SelectLocationRequest>
{
    public SelectLocationUsecase(IRepository repository, IEventBus<DomainEvent> eventBus)
        : base(repository, eventBus)
    {
    }

    public override async Task ExecuteAsync(SelectLocationRequest request)
    {
        //查
        var game = Repository.FindGameById(request.GameId).ToDomain();

        //改
        game.SelectLocation(request.PlayerId, request.LocationID);

        //存
        Repository.Save(game);

        //推
        await EventBus.PublishAsync(game.DomainEvents);
    }
}