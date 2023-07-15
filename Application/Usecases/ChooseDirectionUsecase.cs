using Application.Common;
using Domain.Common;

namespace Application.Usecases;

public record ChooseDirectionRequest(string GameId, string PlayerId, string Direction)
    : Request(GameId, PlayerId);

public class ChooseDirectionUsecase : Usecase<ChooseDirectionRequest>
{
    public ChooseDirectionUsecase(IRepository repository, IEventBus<DomainEvent> eventBus)
        : base(repository, eventBus)
    {
    }

    public override async Task ExecuteAsync(ChooseDirectionRequest request)
    {
        //查
        var game = Repository.FindGameById(request.GameId);
        //改
        game.PlayerSelectDirection(request.PlayerId, request.Direction);
        //存
        Repository.Save(game);
        //推
        await EventBus.PublishAsync(game.DomainEvents);
    }
}