using Application.Usecases;
using Microsoft.AspNetCore.SignalR;

namespace Server.Hubs;

public class MonopolyHub : Hub<IMonopolyResponses>
{
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
}