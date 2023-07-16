using Application.Common;
using Application.Usecases;
using Microsoft.AspNetCore.SignalR;

namespace Server.Hubs;

public class MonopolyHub : Hub<IMonopolyResponses>
{
    private readonly IRepository _repository;

    public async Task CreateGame(string userId, CreateGameUsecase usecase)
    {
        await usecase.ExecuteAsync(new CreateGameRequest(null, userId));
    }

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

    public MonopolyHub(IRepository repository)
    {
        _repository = repository;
    }

    public override Task OnConnectedAsync()
    {
        HttpContext httpContext = Context.GetHttpContext()!;
        var gameId = httpContext.Request.Query["gameid"];
        try
        {
            _repository.FindGameById(gameId);
        }
        catch
        {
            if (gameId.Count == 0)
            {
                throw new Exception("沒有傳遞遊戲Id");
            }
            throw new GameNotFoundException($"找不到ID為{gameId}的遊戲");
        }
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