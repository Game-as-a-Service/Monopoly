using Domain.Events;
using Microsoft.AspNetCore.SignalR;
using Server.Common;
using SharedLibrary;
using SharedLibrary.ResponseArgs.Monopoly;

namespace Server.Hubs.EventHandlers;

public class CurrentPlayerCannotBidEventHandler(IHubContext<MonopolyHub, IMonopolyResponses> hubContext)
    : MonopolyEventHandlerBase<CurrentPlayerCannotBidEvent>
{
    protected override Task HandleSpecificEvent(CurrentPlayerCannotBidEvent e)
    {
        return hubContext.Clients.All.CurrentPlayerCannotBidEvent(
            new CurrentPlayerCannotBidEventArgs
            {
                PlayerId = e.PlayerId
            });
    }
}