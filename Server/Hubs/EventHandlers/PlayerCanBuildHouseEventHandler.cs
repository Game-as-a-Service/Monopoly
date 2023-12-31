using Domain.Events;
using Microsoft.AspNetCore.SignalR;
using Server.Common;
using SharedLibrary;
using SharedLibrary.ResponseArgs.Monopoly;

namespace Server.Hubs.EventHandlers;

public class PlayerCanBuildHouseEventHandler(IHubContext<MonopolyHub, IMonopolyResponses> hubContext)
    : MonopolyEventHandlerBase<PlayerCanBuildHouseEvent>
{
    protected override Task HandleSpecificEvent(PlayerCanBuildHouseEvent e)
    {
        return hubContext.Clients.All.PlayerCanBuildHouseEvent(new PlayerCanBuildHouseEventArgs
        {
            PlayerId = e.PlayerId,
            LandId = e.LandId,
            HouseCount = e.HouseCount,
            Price = e.UpgradePrice
        });
    }
}