using Domain.Events;
using Microsoft.AspNetCore.SignalR;
using Server.Common;
using SharedLibrary;
using SharedLibrary.ResponseArgs.Monopoly;

namespace Server.Hubs.EventHandlers;

public class PlayerTooPoorToBidEventHandler(IHubContext<MonopolyHub, IMonopolyResponses> hubContext)
    : MonopolyEventHandlerBase<PlayerTooPoorToBidEvent>
{
    protected override Task HandleSpecificEvent(PlayerTooPoorToBidEvent e)
    {
        return hubContext.Clients.All.PlayerTooPoorToBidEvent(
            new PlayerTooPoorToBidEventArgs
            {
                PlayerId = e.PlayerId,
                PlayerMoney = e.PlayerMoney,
                BidPrice = e.BidPrice,
                HighestPrice = e.HighestPrice
            });
    }
}