using Domain.Events;
using Microsoft.AspNetCore.SignalR;
using Server.Common;
using SharedLibrary;
using SharedLibrary.ResponseArgs.Monopoly;

namespace Server.Hubs.EventHandlers;

public class PlayerMortgageEventHandler(IHubContext<MonopolyHub, IMonopolyResponses> hubContext)
    : MonopolyEventHandlerBase<PlayerMortgageEvent>
{
    protected override Task HandleSpecificEvent(PlayerMortgageEvent e)
    {
        return hubContext.Clients.All.PlayerMortgageEvent(new PlayerMortgageEventArgs
        {
            PlayerId = e.PlayerId,
            LandId = e.BlockId,
            PlayerMoney = e.PlayerMoney,
            DeadLine = e.DeadLine,
        });
    }
}