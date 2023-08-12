using Application.Common;
using Application.Usecases;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

namespace Server.Hubs;

[Authorize]
public class MonopolyHub : Hub<IMonopolyResponses>
{
    private readonly IRepository _repository;

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

    public MonopolyHub(IRepository repository)
    {
        _repository = repository;
    }

    public override Task OnConnectedAsync()
    {
        HttpContext httpContext = Context.GetHttpContext()!;
        var gameIdStringValues = httpContext.Request.Query["gameid"];
        if (gameIdStringValues.Count == 0)
        {
            Clients.Caller.PlayerJoinGameFailedEvent("Not pass game id");
            Context.Abort();
        }
        string gameId = gameIdStringValues.ToString();
        string userId = Context.User!.FindFirst(x => x.Type == ClaimTypes.Sid)!.Value;
        if (!_repository.IsExist(gameId))
        {
            Clients.Caller.PlayerJoinGameFailedEvent($"Can not find the game that id is {gameId}");
            Context.Abort();
        }
        var game = _repository.FindGameById(gameId);
        if (!game.Players.Any(p => p.Id == userId))
        {
            Clients.Caller.PlayerJoinGameFailedEvent($"Can not find the player whose id is {userId}");
            Context.Abort();
        }
        Groups.AddToGroupAsync(Context.ConnectionId, gameId);
        Clients.Group(gameId).PlayerJoinGameEvent(userId!);
        return base.OnConnectedAsync();
    }

    public class GameNotFoundException : Exception
    {
        public GameNotFoundException(string message)
            : base(message)
        {
        }
    }
}