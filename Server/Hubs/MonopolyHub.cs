using Microsoft.AspNetCore.SignalR;

namespace Server.Hubs;

public class MonopolyHub : Hub
{
    public async Task SendMessage(string user, string message)
    {
        await Clients.All.SendAsync("ReceiveMessage", user, message);
    }

    public async Task PlayerRollDice(string gameId, string userId)
    {
        await Clients.Group(gameId).SendAsync("PlayerRollDice", userId, 6); // TODO: 這裡應該要使用到Usecase
    }
}