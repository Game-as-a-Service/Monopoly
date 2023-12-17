using Application.Common;

namespace Application.Query;

public record GetReadyInfoRequest(string GameId, string PlayerId)
    : Request(GameId, PlayerId);
public record GetReadyInfoResponse(GetReadyInfoResponse.ReadyInfo Info)
{
    public record ReadyInfo(Player[] Players, string HostId);
    public record Player(string PlayerId, bool IsReady, string? RoleId, int? LocationId);
}

public class GetReadyInfosUsecase : QueryUsecase<GetReadyInfoRequest, GetReadyInfoResponse>
{
    public GetReadyInfosUsecase(IRepository repository) : base(repository)
    {
    }

    public override async Task ExecuteAsync(GetReadyInfoRequest request, IPresenter<GetReadyInfoResponse> presenter)
    {
        var game = Repository.FindGameById(request.GameId);

        var players = game.Players.Select(player =>
        {
            var isReady = player.PlayerState == Domain.PlayerState.Ready;
            var roleId = player.RoleId;
            var locationId = player.LocationId;
            return new GetReadyInfoResponse.Player(player.Id, isReady, roleId, locationId);
        }).ToArray();

        var response = new GetReadyInfoResponse(new GetReadyInfoResponse.ReadyInfo(players, game.HostId));

        await presenter.PresentAsync(response);
    }
}
