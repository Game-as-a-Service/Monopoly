using Domain.Events;
using Microsoft.AspNetCore.SignalR;
using Server.Common;
using SharedLibrary;
using SharedLibrary.ResponseArgs.Monopoly;

namespace Server.Hubs.EventHandlers;

public class PlayerReadyEventHandler(IHubContext<MonopolyHub, IMonopolyResponses> hubContext)
    : MonopolyEventHandlerBase<PlayerReadyEvent>
{
    protected override Task HandleSpecificEvent(PlayerReadyEvent e)
    {
        return hubContext.Clients.All.PlayerReadyEvent(new PlayerReadyEventArgs
        {
            PlayerId = e.PlayerId,
            PlayerState = e.PlayerState
        });
    }
}