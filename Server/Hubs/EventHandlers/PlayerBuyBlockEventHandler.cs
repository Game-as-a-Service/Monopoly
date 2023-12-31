using Domain.Events;
using Microsoft.AspNetCore.SignalR;
using Server.Common;
using SharedLibrary;
using SharedLibrary.ResponseArgs.Monopoly;

namespace Server.Hubs.EventHandlers;

public class PlayerBuyBlockEventHandler(IHubContext<MonopolyHub, IMonopolyResponses> hubContext)
    : MonopolyEventHandlerBase<PlayerBuyBlockEvent>
{
    protected override Task HandleSpecificEvent(PlayerBuyBlockEvent e)
    {
        return hubContext.Clients.All.PlayerBuyBlockEvent(
            new PlayerBuyBlockEventArgs
            {
                PlayerId = e.PlayerId,
                LandId = e.LandId
            });
    }
}