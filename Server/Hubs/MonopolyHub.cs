using Application.Common;
using Application.Query;
using Application.Usecases;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Server.Presenters;
using SharedLibrary;
using SharedLibrary.ResponseArgs.Monopoly;
using SharedLibrary.ResponseArgs.ReadyRoom;
using System.Security.Claims;

namespace Server.Hubs;

[Authorize]
public class MonopolyHub(ICommandRepository repository) : Hub<IMonopolyResponses>
{
    private const string Playerid = "PlayerId";
    private const string Gameid = "GameId";
    private string GameId => Context.Items[Gameid] as string ?? "";
    private string PlayerId => Context.Items[Playerid] as string ?? "";

    public async Task PlayerRollDice(string gameId, string userId, PlayerRollDiceUsecase usecase, SignalrDefaultPresenter<PlayerRollDiceResponse> presenter)
    {
        await usecase.ExecuteAsync(
            new PlayerRollDiceRequest(gameId, userId),
            presenter
            );
    }

    public async Task PlayerChooseDirection(string gameId, string userId, string direction, ChooseDirectionUsecase usecase, SignalrDefaultPresenter<ChooseDirectionResponse> presenter)
    {
        await usecase.ExecuteAsync(
            new ChooseDirectionRequest(gameId, userId, direction),
            presenter
            );
    }

    public async Task PlayerBuyLand(string gameId, string userId, string blockId, PlayerBuyLandUsecase usecase, SignalrDefaultPresenter<PlayerBuyLandResponse> presenter)
    {
        await usecase.ExecuteAsync(
            new PlayerBuyLandRequest(gameId, userId, blockId),
            presenter
            );
    }

    public async Task PlayerPayToll(string gameId, string userId, PayTollUsecase usecase, SignalrDefaultPresenter<PayTollResponse> presenter)
    {
        await usecase.ExecuteAsync(
            new PayTollRequest(gameId, userId),
            presenter
            );
    }

    public async Task PlaySelectLocation(string gameId, string userId, int locationId, SelectLocationUsecase usecase, SignalrDefaultPresenter<SelectLocationResponse> presenter)
    {
        await usecase.ExecuteAsync(
            new SelectLocationRequest(gameId, userId, locationId),
            presenter
            );
    }

    public async Task PlayerBuildHouse(string gameId, string userId, BuildHouseUsecase usecase, SignalrDefaultPresenter<BuildHouseResponse> presenter)
    {
        await usecase.ExecuteAsync(
            new BuildHouseRequest(gameId, userId),
            presenter
            );
    }

    public async Task PlayerMortgage(string gameId, string userId, string blockId, MortgageUsecase usecase, SignalrDefaultPresenter<MortgageResponse> presenter)
    {
        await usecase.ExecuteAsync(
            new MortgageRequest(gameId, userId, blockId),
            presenter
            );
    }

    public async Task PlayerRedeem(string gameId, string userId, string blockId, RedeemUsecase usecase, SignalrDefaultPresenter<RedeemResponse> presenter)
    {
        await usecase.ExecuteAsync(
            new RedeemRequest(gameId, userId, blockId),
            presenter
            );
    }

    public async Task PlayerBid(string gameId, string userId, decimal bidPrice, BidUsecase usecase, SignalrDefaultPresenter<BidResponse> presenter)
    {
        await usecase.ExecuteAsync(
            new BidRequest(gameId, userId, bidPrice),
            presenter
            );
    }

    public async Task EndAuction(string gameId, string userId, EndAuctionUsecase usecase, SignalrDefaultPresenter<EndAuctionResponse> presenter)
    {
        await usecase.ExecuteAsync(
            new EndAuctionRequest(gameId, userId),
            presenter
            );
    }

    public async Task EndRound(string gameId, string userId, EndRoundUsecase usecase, SignalrDefaultPresenter<EndRoundResponse> presenter)
    {
        await usecase.ExecuteAsync(
            new EndRoundRequest(gameId, userId),
            presenter
            );
    }

    public async Task Settlement(string gameId, string userId, SettlementUsecase usecase, SignalrDefaultPresenter<SettlementResponse> presenter)
    {
        await usecase.ExecuteAsync(
            new SettlementRequest(gameId, userId),
            presenter
            );
    }

    public async Task PlayerSelectRole(string gameId, string userId, string roleId, SelectRoleUsecase usecase, SignalrDefaultPresenter<SelectRoleResponse> presenter)
    {
        await usecase.ExecuteAsync(
            new SelectRoleRequest(gameId, userId, roleId),
            presenter
            );
    }

    public async Task PlayerReady(string gameId, string userId, PlayerPreparedUsecase usecase, SignalrDefaultPresenter<PlayerPreparedResponse> presenter)
    {
        await usecase.ExecuteAsync(
            new PlayerPreparedRequest(gameId, userId),
            presenter
            );
    }

    public async Task GameStart(string gameId, string userId, GameStartUsecase usecase, SignalrDefaultPresenter<GameStartResponse> presenter)
    {
        await usecase.ExecuteAsync(
            new GameStartRequest(gameId, userId),
            presenter
            );
    }

    public async Task GetReadyInfo(GetReadyInfoUsecase usecase)
    {
        var presenter = new DefaultPresenter<GetReadyInfoResponse>();
        await usecase.ExecuteAsync(new GetReadyInfoRequest(GameId, PlayerId), presenter);
        await Clients.Caller.GetReadyInfoEvent(new GetReadyInfoEventArgs
        {
            Players = presenter.Value.Info.Players.Select(x =>
            {
                var isRole = Enum.TryParse<GetReadyInfoEventArgs.RoleEnum>(x.RoleId, true, out var roleEnum);
                if (isRole is false)
                {
                    roleEnum = GetReadyInfoEventArgs.RoleEnum.None;
                }
                return new GetReadyInfoEventArgs.Player
                {
                    Id = x.PlayerId,
                    Name = x.PlayerId,
                    IsReady = x.IsReady,
                    Role = roleEnum,
                    Color = (GetReadyInfoEventArgs.ColorEnum?)x.LocationId ?? GetReadyInfoEventArgs.ColorEnum.None
                };
            }).ToList(),
            HostId = presenter.Value.Info.HostId,
        });
    }

    public override async Task OnConnectedAsync()
    {
        var httpContext = Context.GetHttpContext()!;
        var gameIdStringValues = httpContext.Request.Query["gameId"];
        if (gameIdStringValues.Count is 0)
        {
            throw new GameNotFoundException($"Not pass game id");
        }
        Context.Items[Gameid] = gameIdStringValues.ToString();
        Context.Items[Playerid] = Context.User!.FindFirst(x => x.Type == ClaimTypes.Sid)!.Value;
        if (repository.IsExist(GameId) is false)
        {
            throw new GameNotFoundException($"Can not find the game that id is {GameId}");
        }
        await Groups.AddToGroupAsync(Context.ConnectionId, GameId);
        await Clients.Caller.WelcomeEvent(new WelcomeEventArgs { PlayerId = PlayerId });
        await Clients.Group(GameId).PlayerJoinGameEvent(new PlayerJoinGameEventArgs { PlayerId = PlayerId });
    }

    private class GameNotFoundException(string message) : Exception(message);
}