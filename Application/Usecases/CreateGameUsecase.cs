using Application.Common;
using Domain.Builders;
using Domain.Common;
using Domain.Maps;

namespace Application.Usecases;

public record CreateGameRequest(string HostId, string[] PlayerIds) : Request(null!, HostId);

public record CreateGameResponse(string GameId) : Response;

public class CreateGameUsecase(IRepository repository)
    : Usecase<CreateGameRequest, CreateGameResponse>(repository)
{
    public override async Task ExecuteAsync(CreateGameRequest request, IPresenter<CreateGameResponse> presenter)
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
        var id = Repository.Save(builder.Build());

        // 推
        await presenter.PresentAsync(new CreateGameResponse(id));
    }
}