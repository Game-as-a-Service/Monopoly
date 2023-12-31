using Domain.Events;
using Microsoft.AspNetCore.SignalR;
using Server.Common;
using SharedLibrary;
using SharedLibrary.ResponseArgs.Monopoly;

namespace Server.Hubs.EventHandlers;

public class MortgageDueEventHandler(IHubContext<MonopolyHub, IMonopolyResponses> hubContext)
    : MonopolyEventHandlerBase<MortgageDueEvent>
{
    protected override Task HandleSpecificEvent(MortgageDueEvent e)
    {
        return hubContext.Clients.All.MortgageDueEvent(
            new MortgageDueEventArgs
            {
                PlayerId = e.PlayerId,
                LandId = e.LandId
            });
    }
}