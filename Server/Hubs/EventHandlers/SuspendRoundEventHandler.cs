using Domain.Events;
using Microsoft.AspNetCore.SignalR;
using Server.Common;
using SharedLibrary;
using SharedLibrary.ResponseArgs.Monopoly;

namespace Server.Hubs.EventHandlers;

public class SuspendRoundEventHandler(IHubContext<MonopolyHub, IMonopolyResponses> hubContext)
    : MonopolyEventHandlerBase<SuspendRoundEvent>
{
    protected override Task HandleSpecificEvent(SuspendRoundEvent e)
    {
        return hubContext.Clients.All.SuspendRoundEvent(new SuspendRoundEventArgs
        {
            PlayerId = e.PlayerId,
            SuspendRounds = e.SuspendRounds
        });
    }
}