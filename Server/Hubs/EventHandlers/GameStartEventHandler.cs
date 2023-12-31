using Domain.Events;
using Microsoft.AspNetCore.SignalR;
using Server.Common;
using SharedLibrary;
using SharedLibrary.ResponseArgs.Monopoly;

namespace Server.Hubs.EventHandlers;

public class GameStartEventHandler(IHubContext<MonopolyHub, IMonopolyResponses> hubContext)
    : MonopolyEventHandlerBase<GameStartEvent>
{
    protected override Task HandleSpecificEvent(GameStartEvent e)
    {
        return hubContext.Clients.All.GameStartEvent(new GameStartEventArgs
        {
            GameStage = e.GameStage,
            CurrentPlayerId = e.CurrentPlayerId,
        });
    }
}