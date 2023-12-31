using Domain.Events;
using Microsoft.AspNetCore.SignalR;
using Server.Common;
using SharedLibrary;
using SharedLibrary.ResponseArgs.Monopoly;

namespace Server.Hubs.EventHandlers;

public class PlayerBidFailEventHandler(IHubContext<MonopolyHub, IMonopolyResponses> hubContext)
    : MonopolyEventHandlerBase<PlayerBidFailEvent>
{
    protected override Task HandleSpecificEvent(PlayerBidFailEvent e)
    {
        return hubContext.Clients.All.PlayerBidFailEvent(
            new PlayerBidFailEventArgs
            {
                PlayerId = e.PlayerId,
                LandId = e.LandId,
                BidPrice = e.BidPrice,
                HighestPrice = e.HighestPrice
            });
    }
}