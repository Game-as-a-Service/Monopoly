using Domain.Events;
using Microsoft.AspNetCore.SignalR;
using Server.Common;
using SharedLibrary;
using SharedLibrary.ResponseArgs.Monopoly;

namespace Server.Hubs.EventHandlers;

public class EndRoundEventHandler(IHubContext<MonopolyHub, IMonopolyResponses> hubContext)
    : MonopolyEventHandlerBase<EndRoundEvent>
{
    protected override Task HandleSpecificEvent(EndRoundEvent e)
    {
        return hubContext.Clients.All.EndRoundEvent(new EndRoundEventArgs
        {
            PlayerId = e.PlayerId,
            NextPlayerId = e.NextPlayerId
        });
    }
}