using Domain.Events;
using Microsoft.AspNetCore.SignalR;
using Server.Common;
using SharedLibrary;
using SharedLibrary.ResponseArgs.Monopoly;

namespace Server.Hubs.EventHandlers;

public class PlayerBuildHouseEventHandler(IHubContext<MonopolyHub, IMonopolyResponses> hubContext)
    : MonopolyEventHandlerBase<PlayerBuildHouseEvent>
{
    protected override Task HandleSpecificEvent(PlayerBuildHouseEvent e)
    {
        return hubContext.Clients.All.PlayerBuildHouseEvent(
            new PlayerBuildHouseEventArgs
            {
                PlayerId = e.PlayerId,
                LandId = e.LandId,
                PlayerMoney = e.PlayerMoney,
                HouseCount = e.HouseCount
            }
        );
    }
}