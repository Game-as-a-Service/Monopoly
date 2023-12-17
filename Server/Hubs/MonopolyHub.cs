using Application.Common;
using Application.Query;
using Application.Usecases;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Server.Presenters;
using SharedLibrary;
using SharedLibrary.ResponseArgs;
using System.Security.Claims;

namespace Server.Hubs;

[Authorize]
public class MonopolyHub : Hub<IMonopolyResponses>
{
    private readonly IRepository _repository;
    private string GameId => Context.Items["GameId"] as string ?? "";
    private string UserId => Context.Items["UserId"] as string ?? "";

    public async Task PlayerRollDice(string gameId, string userId, RollDiceUsecase usecase)
    {
        await usecase.ExecuteAsync(new RollDiceRequest(gameId, userId));
    }

    public async Task PlayerChooseDirection(string gameId, string userId, string direction, ChooseDirectionUsecase usecase)
    {
        await usecase.ExecuteAsync(new ChooseDirectionRequest(gameId, userId, direction));
    }

    public async Task PlayerBuyLand(string gameId, string userId, string blockId, BuyBlockUsecase usecase)
    {
        await usecase.ExecuteAsync(new BuyBlockRequest(gameId, userId, blockId));
    }

    public async Task PlayerPayToll(string gameId, string userId, PayTollUsecase usecase)
    {
        await usecase.ExecuteAsync(new PayTollRequest(gameId, userId));
    }

    public async Task PlaySelectLocation(string gameId, string userId, int LocationID, SelectLocationUsecase usecase)
    {
        await usecase.ExecuteAsync(new SelectLocationRequest(gameId, userId, LocationID));
    }

    public async Task PlayerBuildHouse(string gameId, string userId, BuildHouseUsecase usecase)
    {
        await usecase.ExecuteAsync(new BuildHouseRequest(gameId, userId));
    }

    public async Task PlayerMortgage(string gameId, string userId, string blockId, MortgageUsecase usecase)
    {
        await usecase.ExecuteAsync(new MortgageRequest(gameId, userId, blockId));
    }

    public async Task PlayerRedeem(string gameId, string userId, string blockId, RedeemUsecase usecase)
    {
        await usecase.ExecuteAsync(new RedeemRequest(gameId, userId, blockId));
    }

    public async Task PlayerBid(string gameId, string userId, decimal bidPrice, BidUsecase usecase)
    {
        await usecase.ExecuteAsync(new BidRequest(gameId, userId, bidPrice));
    }

    public async Task EndAuction(string gameId, string userId, EndAuctionUsecase usecase)
    {
        await usecase.ExecuteAsync(new EndAuctionRequest(gameId, userId));
    }

    public async Task EndRound(string gameId, string userId, EndRoundUsecase usecase)
    {
        await usecase.ExecuteAsync(new EndRoundRequest(gameId, userId));
    }

    public async Task Settlement(string gameId, string userId, SettlementUsecase usecase)
    {
        await usecase.ExecuteAsync(new SettlementRequest(gameId, userId));
    }

    public async Task PlayerSelectRole(string gameId, string userId, string roleId, SelectRoleUsecase usecase)
    {
        await usecase.ExecuteAsync(new SelectRoleRequest(gameId, userId, roleId));
    }

    public async Task PlayerReady(string gameId, string userId, PlayerPreparedUsecase usecase)
    {
        await usecase.ExecuteAsync(new PlayerPreparedRequest(gameId, userId));
    }

    public async Task GameStart(string gameId, string userId, GameStartUsecase usecase)
    {
        await usecase.ExecuteAsync(new GameStartRequest(gameId, userId));
    }

    public async Task GetReadyInfo(GetReadyInfosUsecase usecase)
    {
        var presenter = new DefaultPresenter<GetReadyInfoResponse>();
        await usecase.ExecuteAsync(new GetReadyInfoRequest(GameId, UserId), presenter);
        await Clients.Caller.GetReadyInfoEvent(new()
        {
            Players = presenter.Value.Info.Players.Select(x =>
            {
                var isRole = Enum.TryParse<GetReadyInfoEvent.RoleEnum>(x.RoleId, true, out var roleEnum);
                if (isRole is false)
                {
                    roleEnum = GetReadyInfoEvent.RoleEnum.None;
                }
                return new GetReadyInfoEvent.Player
                {
                    Id = x.PlayerId,
                    Name = x.PlayerId,
                    IsReady = x.IsReady,
                    Role = roleEnum,
                    Color = (GetReadyInfoEvent.ColorEnum?)x.LocationId ?? GetReadyInfoEvent.ColorEnum.None
                };
            }).ToList(),
            HostId = presenter.Value.Info.HostId,
        });
    }

    public MonopolyHub(IRepository repository)
    {
        _repository = repository;
    }

    public override async Task OnConnectedAsync()
    {
        HttpContext httpContext = Context.GetHttpContext()!;
        var gameIdStringValues = httpContext.Request.Query["gameid"];
        if (gameIdStringValues.Count == 0)
        {
            throw new GameNotFoundException($"Not pass game id");
        }
        Context.Items["GameId"] = gameIdStringValues.ToString();
        Context.Items["UserId"] = Context.User!.FindFirst(x => x.Type == ClaimTypes.Sid)!.Value;
        if (!_repository.IsExist(GameId))
        {
            throw new GameNotFoundException($"Can not find the game that id is {GameId}");
        }
        await Groups.AddToGroupAsync(Context.ConnectionId, GameId);
        await Clients.Caller.WelcomeEvent(new() { PlayerId = UserId });
        await Clients.Group(GameId).PlayerJoinGameEvent(UserId!);
    }

    public class GameNotFoundException : Exception
    {
        public GameNotFoundException(string message)
            : base(message)
        {
        }
    }
}