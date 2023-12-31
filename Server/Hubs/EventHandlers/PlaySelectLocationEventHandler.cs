using Domain.Events;
using Microsoft.AspNetCore.SignalR;
using Server.Common;
using SharedLibrary;
using SharedLibrary.ResponseArgs.Monopoly;

namespace Server.Hubs.EventHandlers;

public class PlaySelectLocationEventHandler(IHubContext<MonopolyHub, IMonopolyResponses> hubContext)
    : MonopolyEventHandlerBase<PlaySelectLocationEvent>
{
    protected override Task HandleSpecificEvent(PlaySelectLocationEvent e)
    {
        return hubContext.Clients.All.PlaySelectLocationEvent(new PlaySelectLocationEventArgs
        {
            PlayerId = e.PlayerId,
            LocationId = e.LocationId,
        });
    }
}