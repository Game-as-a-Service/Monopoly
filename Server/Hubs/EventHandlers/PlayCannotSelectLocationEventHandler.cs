using Domain.Events;
using Microsoft.AspNetCore.SignalR;
using Server.Common;
using SharedLibrary;
using SharedLibrary.ResponseArgs.Monopoly;

namespace Server.Hubs.EventHandlers;

public class PlayCannotSelectLocationEventHandler(IHubContext<MonopolyHub, IMonopolyResponses> hubContext)
    : MonopolyEventHandlerBase<PlayCannotSelectLocationEvent>
{
    protected override Task HandleSpecificEvent(PlayCannotSelectLocationEvent e)
    {
        return hubContext.Clients.All.PlayCannotSelectLocationEvent(new PlayCannotSelectLocationEventArgs
        {
            PlayerId = e.PlayerId,
            LocationId = e.LocationId
        });
    }
}