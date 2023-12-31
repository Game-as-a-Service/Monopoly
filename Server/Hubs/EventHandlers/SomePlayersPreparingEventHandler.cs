using Domain.Events;
using Microsoft.AspNetCore.SignalR;
using Server.Common;
using SharedLibrary;
using SharedLibrary.ResponseArgs.Monopoly;

namespace Server.Hubs.EventHandlers;

public class SomePlayersPreparingEventHandler(IHubContext<MonopolyHub, IMonopolyResponses> hubContext)
    : MonopolyEventHandlerBase<SomePlayersPreparingEvent>
{
    protected override Task HandleSpecificEvent(SomePlayersPreparingEvent e)
    {
        return hubContext.Clients.All.SomePlayersPreparingEvent(new SomePlayersPreparingEventArgs
        {
            GameStage = e.GameStage,
            PlayerIds = e.PlayerIds,
        });
    }
}