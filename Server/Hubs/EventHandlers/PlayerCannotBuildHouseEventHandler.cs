using Domain.Events;
using Microsoft.AspNetCore.SignalR;
using Server.Common;
using SharedLibrary;
using SharedLibrary.ResponseArgs.Monopoly;

namespace Server.Hubs.EventHandlers;

public class PlayerCannotBuildHouseEventHandler(IHubContext<MonopolyHub, IMonopolyResponses> hubContext)
    : MonopolyEventHandlerBase<PlayerCannotBuildHouseEvent>
{
    protected override Task HandleSpecificEvent(PlayerCannotBuildHouseEvent e)
    {
        return hubContext.Clients.All.PlayerCannotBuildHouseEvent(
            new PlayerCannotBuildHouseEventArgs
            {
                PlayerId = e.PlayerId,
                LandId = e.LandId
            });
    }
}