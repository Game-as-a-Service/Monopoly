using Domain.Events;
using Microsoft.AspNetCore.SignalR;
using Server.Common;
using SharedLibrary;
using SharedLibrary.ResponseArgs.Monopoly;

namespace Server.Hubs.EventHandlers;

public class PlayerBidEventHandler(IHubContext<MonopolyHub, IMonopolyResponses> hubContext)
    : MonopolyEventHandlerBase<PlayerBidEvent>
{
    protected override Task HandleSpecificEvent(PlayerBidEvent e)
    {
        return hubContext.Clients.All.PlayerBidEvent(
            new PlayerBidEventArgs
            {
                PlayerId = e.PlayerId,
                LandId = e.LandId,
                HighestPrice = e.HighestPrice
            });
    }
}