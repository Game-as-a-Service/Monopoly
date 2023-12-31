using Domain.Events;
using Microsoft.AspNetCore.SignalR;
using Server.Common;
using SharedLibrary;
using SharedLibrary.ResponseArgs.Monopoly;

namespace Server.Hubs.EventHandlers;

public class PlayerDoesntNeedToPayTollEventHandler(IHubContext<MonopolyHub, IMonopolyResponses> hubContext)
    : MonopolyEventHandlerBase<PlayerDoesntNeedToPayTollEvent>
{
    protected override Task HandleSpecificEvent(PlayerDoesntNeedToPayTollEvent e)
    {
        return hubContext.Clients.All.PlayerDoesntNeedToPayTollEvent(new PlayerDoesntNeedToPayTollEventArgs
        {
            PlayerId = e.PlayerId,
            PlayerMoney = e.PlayerMoney
        });
    }
}