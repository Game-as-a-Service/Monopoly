using Application.Common;
using Domain;
using Domain.Common;
using Domain.Events;

namespace Application.Usecases;

public record CreateGameRequest(string GameId, string PlayerId) : Request(GameId, PlayerId);

public class CreateGameUsecase : Usecase<CreateGameRequest>
{
    public CreateGameUsecase(IRepository repository, IEventBus<DomainEvent> eventBus) : base(repository, eventBus)
    {
    }

    public override async Task ExecuteAsync(CreateGameRequest request)
    {
        // 查
        // 改
        Monopoly game = new(request.GameId);

        // 存
        string id = Repository.Save(game);
        List<DomainEvent> domainEvents = game.DomainEvents.ToList();
        domainEvents.Add(new GameCreatedEvent(id));

        // 推
        await EventBus.PublishAsync(domainEvents);
    }
}