using Application.Common;
using Domain;
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
        Monopoly game = new(request.GameId);
        request.PlayerIds.ToList().ForEach(playerId =>
        {
            Player player = new(playerId);
            if (playerId == request.HostId)
            {
                player.IsHost = true;
            }
            game.AddPlayer(player);
        });

        // 存
        string id = Repository.Save(game);

        return id;
    }
}