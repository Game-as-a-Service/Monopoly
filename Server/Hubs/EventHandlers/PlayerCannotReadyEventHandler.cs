using Domain.Events;
using Microsoft.AspNetCore.SignalR;
using Server.Common;
using SharedLibrary;
using SharedLibrary.ResponseArgs.Monopoly;

namespace Server.Hubs.EventHandlers;

public class PlayerCannotReadyEventHandler(IHubContext<MonopolyHub, IMonopolyResponses> hubContext)
    : MonopolyEventHandlerBase<PlayerCannotReadyEvent>
{
    protected override Task HandleSpecificEvent(PlayerCannotReadyEvent e)
    {
        return hubContext.Clients.All.PlayerCannotReadyEvent(new PlayerCannotReadyEventArgs
        {
            PlayerId = e.PlayerId,
            PlayerState = e.PlayerState,
            RoleId = e.RoleId,
            LocationId = e.LocationId
        });
    }
}