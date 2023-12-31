using Domain.Events;
using Microsoft.AspNetCore.SignalR;
using Server.Common;
using SharedLibrary;
using SharedLibrary.ResponseArgs.Monopoly;

namespace Server.Hubs.EventHandlers;

public class EndRoundFailEventHandler(IHubContext<MonopolyHub, IMonopolyResponses> hubContext)
    : MonopolyEventHandlerBase<EndRoundFailEvent>
{
    protected override Task HandleSpecificEvent(EndRoundFailEvent e)
    {
        return hubContext.Clients.All.EndRoundFailEvent(new EndRoundFailEventArgs
        {
            PlayerId = e.PlayerId,
        });
    }
}