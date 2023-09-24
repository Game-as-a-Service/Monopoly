using Application.Common;
using Domain.Builders;
using Domain.Common;

namespace Application.Usecases;

public record CreateGameRequest(string HostId, string[] PlayerIds) : Request(null!, HostId);

public class CreateGameUsecase : Usecase<CreateGameRequest>
{
    public CreateGameUsecase(IRepository repository, IEventBus<DomainEvent> eventBus) : base(repository, eventBus)
    {
    }

    public override async Task ExecuteAsync(CreateGameRequest request)
    {
        throw new NotImplementedException();
    }

    public string Execute(CreateGameRequest request)
    {
        // 查
        // 改

        var builder = new MonopolyBuilder();
        foreach (var playerId in request.PlayerIds)
        {
            builder.WithPlayer(playerId);
        }
        builder.WithHost(request.HostId);

        // 存
        string id = Repository.Save(builder.Build());

        return id;
    }
}