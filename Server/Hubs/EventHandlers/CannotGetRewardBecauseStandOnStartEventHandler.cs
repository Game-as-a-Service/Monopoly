using Domain.Events;
using Microsoft.AspNetCore.SignalR;
using Server.Common;
using SharedLibrary;
using SharedLibrary.ResponseArgs.Monopoly;

namespace Server.Hubs.EventHandlers;

public class CannotGetRewardBecauseStandOnStartEventHandler(IHubContext<MonopolyHub, IMonopolyResponses> hubContext)
    : MonopolyEventHandlerBase<CannotGetRewardBecauseStandOnStartEvent>
{
    protected override Task HandleSpecificEvent(CannotGetRewardBecauseStandOnStartEvent e)
    {
        return hubContext.Clients.All.CannotGetRewardBecauseStandOnStartEvent(
            new CannotGetRewardBecauseStandOnStartEventArgs
            {
                PlayerId = e.PlayerId
            });
    }
}