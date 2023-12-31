using Domain.Events;
using Microsoft.AspNetCore.SignalR;
using Server.Common;
using SharedLibrary;
using SharedLibrary.ResponseArgs.Monopoly;

namespace Server.Hubs.EventHandlers;

public class HouseMaxEventHandler(IHubContext<MonopolyHub, IMonopolyResponses> hubContext)
    : MonopolyEventHandlerBase<HouseMaxEvent>
{
    protected override Task HandleSpecificEvent(HouseMaxEvent e)
    {
        return hubContext.Clients.All.HouseMaxEvent(
            new HouseMaxEventArgs
            {
                PlayerId = e.PlayerId,
                LandId = e.LandId,
                HouseCount = e.HouseCount
            });
    }
}