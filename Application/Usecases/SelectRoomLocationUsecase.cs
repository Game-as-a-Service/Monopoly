using Application.Common;
using Domain.Common;

namespace Application.Usecases;

public record SelectRoomLocationRequest(string GameId, string PlayerId, int LocationID)
    : Request(GameId, PlayerId);

public class SelectRoomLocationUsecase : Usecase<SelectRoomLocationRequest>
{
    public SelectRoomLocationUsecase(IRepository repository, IEventBus<DomainEvent> eventBus)
        : base(repository, eventBus)
    {
    }

    public override async Task ExecuteAsync(SelectRoomLocationRequest request)
    {
        //查
        var game = Repository.FindGameById(request.GameId).ToDomain();

        //改
        game.SelectRoomLocation(request.PlayerId, request.LocationID);

        //存
        Repository.Save(game);

        //推
        await EventBus.PublishAsync(game.DomainEvents);
    }
}