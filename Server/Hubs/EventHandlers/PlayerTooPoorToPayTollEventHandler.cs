using Domain.Events;
using Microsoft.AspNetCore.SignalR;
using Server.Common;
using SharedLibrary;
using SharedLibrary.ResponseArgs.Monopoly;

namespace Server.Hubs.EventHandlers;

public class PlayerTooPoorToPayTollEventHandler(IHubContext<MonopolyHub, IMonopolyResponses> hubContext)
    : MonopolyEventHandlerBase<PlayerTooPoorToPayTollEvent>
{
    protected override Task HandleSpecificEvent(PlayerTooPoorToPayTollEvent e)
    {
        return hubContext.Clients.All.PlayerTooPoorToPayTollEvent(new PlayerTooPoorToPayTollEventArgs
        {
            PlayerId = e.PlayerId,
            PlayerMoney = e.PlayerMoney,
            Toll = e.Toll,
        });
    }
}