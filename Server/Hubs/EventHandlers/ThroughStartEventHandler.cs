using Domain.Events;
using Microsoft.AspNetCore.SignalR;
using Server.Common;
using SharedLibrary;
using SharedLibrary.ResponseArgs.Monopoly;

namespace Server.Hubs.EventHandlers;

public class ThroughStartEventHandler(IHubContext<MonopolyHub, IMonopolyResponses> hubContext) : MonopolyEventHandlerBase<ThroughStartEvent>
{
    protected override Task HandleSpecificEvent(ThroughStartEvent e)
    {
        return hubContext.Clients.All.ThroughStartEvent(new PlayerThroughStartEventArgs
        {
            PlayerId = e.PlayerId,
            GainMoney = e.GainMoney,
            TotalMoney = e.TotalMoney
        });
    }
}