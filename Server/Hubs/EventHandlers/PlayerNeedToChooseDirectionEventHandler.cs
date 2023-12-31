using Domain.Events;
using Microsoft.AspNetCore.SignalR;
using Server.Common;
using SharedLibrary;
using SharedLibrary.ResponseArgs.Monopoly;

namespace Server.Hubs.EventHandlers;

public class PlayerNeedToChooseDirectionEventHandler(IHubContext<MonopolyHub, IMonopolyResponses> hubContext)
    : MonopolyEventHandlerBase<PlayerNeedToChooseDirectionEvent>
{
    protected override Task HandleSpecificEvent(PlayerNeedToChooseDirectionEvent e)
    {
        return hubContext.Clients.All.PlayerNeedToChooseDirectionEvent(
            new PlayerNeedToChooseDirectionEventArgs
            {
                PlayerId = e.PlayerId,
                Directions = e.Directions
            });
    }
}