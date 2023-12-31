using Domain.Events;
using Microsoft.AspNetCore.SignalR;
using Server.Common;
using SharedLibrary;
using SharedLibrary.ResponseArgs.Monopoly;

namespace Server.Hubs.EventHandlers;

public class PlayerPayTollEventHandler(IHubContext<MonopolyHub, IMonopolyResponses> hubContext)
    : MonopolyEventHandlerBase<PlayerPayTollEvent>
{
    protected override Task HandleSpecificEvent(PlayerPayTollEvent e)
    {
        return hubContext.Clients.All.PlayerPayTollEvent(new PlayerPayTollEventArgs
        {
            PlayerId = e.PlayerId,
            PlayerMoney = e.PlayerMoney,
            OwnerId = e.OwnerId,
            OwnerMoney = e.OwnerMoney,
        });
    }
}