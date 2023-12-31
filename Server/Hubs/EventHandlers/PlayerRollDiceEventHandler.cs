using Domain.Common;
using Domain.Events;
using Microsoft.AspNetCore.SignalR;
using Server.Common;
using SharedLibrary;
using PlayerRolledDiceEventArgs = SharedLibrary.ResponseArgs.Monopoly.PlayerRolledDiceEventArgs;

namespace Server.Hubs.EventHandlers;

public class PlayerRollDiceEventHandler(IHubContext<MonopolyHub, IMonopolyResponses> hubContext) 
    : MonopolyEventHandlerBase<PlayerRolledDiceEvent>
{
    protected override Task HandleSpecificEvent(PlayerRolledDiceEvent e)
    {
        return hubContext.Clients.All.PlayerRolledDiceEvent(new PlayerRolledDiceEventArgs
        {
            PlayerId = e.PlayerId,
            DiceCount = e.DiceCount
        });
    }
}