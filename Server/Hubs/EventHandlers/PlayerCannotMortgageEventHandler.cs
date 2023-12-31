using Domain.Events;
using Microsoft.AspNetCore.SignalR;
using Server.Common;
using SharedLibrary;
using SharedLibrary.ResponseArgs.Monopoly;

namespace Server.Hubs.EventHandlers;

public class PlayerCannotMortgageEventHandler(IHubContext<MonopolyHub, IMonopolyResponses> hubContext)
    : MonopolyEventHandlerBase<PlayerCannotMortgageEvent>
{
    protected override Task HandleSpecificEvent(PlayerCannotMortgageEvent e)
    {
        return hubContext.Clients.All.PlayerCannotMortgageEvent(new PlayerCannotMortgageEventArgs
        {
            PlayerId = e.PlayerId,
            LandId = e.LandId,
            PlayerMoney = e.PlayerMoney,
        });
    }
}