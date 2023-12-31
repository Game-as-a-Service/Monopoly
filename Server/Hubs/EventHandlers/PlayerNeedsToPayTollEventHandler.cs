using Domain.Events;
using Microsoft.AspNetCore.SignalR;
using Server.Common;
using SharedLibrary;

namespace Server.Hubs.EventHandlers;

public class PlayerNeedsToPayTollEventHandler(IHubContext<MonopolyHub, IMonopolyResponses> hubContext)
    : MonopolyEventHandlerBase<PlayerNeedsToPayTollEvent>
{
    protected override Task HandleSpecificEvent(PlayerNeedsToPayTollEvent e)
    {
        return hubContext.Clients.All.PlayerNeedsToPayTollEvent(new PlayerNeedsToPayTollEventArgs
        { 
            PlayerId = e.PlayerId,
            OwnerId = e.OwnerId,
            Toll = e.Toll
        });
    }
}