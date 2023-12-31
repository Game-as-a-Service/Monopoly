using Domain.Events;
using Microsoft.AspNetCore.SignalR;
using Server.Common;
using SharedLibrary;
using SharedLibrary.ResponseArgs.Monopoly;

namespace Server.Hubs.EventHandlers;

public class PlayerCanBuyLandEventHandler(IHubContext<MonopolyHub, IMonopolyResponses> hubContext) : MonopolyEventHandlerBase<PlayerCanBuyLandEvent>
{
    protected override Task HandleSpecificEvent(PlayerCanBuyLandEvent e)
    {
        return hubContext.Clients.All.PlayerCanBuyLandEvent(new PlayerCanBuyLandEventArgs
        {
            PlayerId = e.PlayerId,
            LandId = e.LandId,
            Price = e.Price
        });
    }
}