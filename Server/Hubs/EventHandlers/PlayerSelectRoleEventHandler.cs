using Domain.Events;
using Microsoft.AspNetCore.SignalR;
using Server.Common;
using SharedLibrary;
using SharedLibrary.ResponseArgs.Monopoly;

namespace Server.Hubs.EventHandlers;

public class PlayerSelectRoleEventHandler(IHubContext<MonopolyHub, IMonopolyResponses> hubContext)
    : MonopolyEventHandlerBase<PlayerSelectRoleEvent>
{
    protected override Task HandleSpecificEvent(PlayerSelectRoleEvent e)
    {
        return hubContext.Clients.All.PlayerSelectRoleEvent(new PlayerSelectRoleEventArgs
        {
            PlayerId = e.PlayerId,
            RoleId = e.RoleId,
        });
    }
}