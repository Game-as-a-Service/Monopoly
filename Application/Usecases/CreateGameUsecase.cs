using Application.Common;
using Domain.Builders;
using Domain.Common;
using Domain.Maps;

namespace Application.Usecases;

public record CreateGameRequest(string HostId, string[] PlayerIds) : Request(null!, HostId);

public class CreateGameUsecase : Usecase<CreateGameRequest>
{
    public CreateGameUsecase(IRepository repository, IEventBus<DomainEvent> eventBus) : base(repository, eventBus)
    {
    }

#pragma warning disable CS1998 // Async 方法缺乏 'await' 運算子，將同步執行
    public override async Task ExecuteAsync(CreateGameRequest request)
#pragma warning restore CS1998 // Async 方法缺乏 'await' 運算子，將同步執行
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
        builder.WithCurrentPlayer(request.PlayerIds.First());
        builder.WithMap(new SevenXSevenMap());

        // 存
        string id = Repository.Save(builder.Build());

        return id;
    }
}