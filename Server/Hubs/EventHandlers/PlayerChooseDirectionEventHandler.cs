using Domain.Events;
using Microsoft.AspNetCore.SignalR;
using Server.Common;
using SharedLibrary;
using SharedLibrary.ResponseArgs.Monopoly;

namespace Server.Hubs.EventHandlers;

public class PlayerChooseDirectionEventHandler(IHubContext<MonopolyHub, IMonopolyResponses> hubContext)
    : MonopolyEventHandlerBase<PlayerChooseDirectionEvent>
{
    protected override Task HandleSpecificEvent(PlayerChooseDirectionEvent e)
    {
        return hubContext.Clients.All.PlayerChooseDirectionEvent(new PlayerChooseDirectionEventArgs
        {
            PlayerId = e.PlayerId,
            Direction = e.Direction,
        });
    }
}