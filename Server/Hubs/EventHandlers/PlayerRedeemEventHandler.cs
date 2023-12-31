using Domain.Events;
using Microsoft.AspNetCore.SignalR;
using Server.Common;
using SharedLibrary;
using SharedLibrary.ResponseArgs.Monopoly;

namespace Server.Hubs.EventHandlers;

public class PlayerRedeemEventHandler(IHubContext<MonopolyHub, IMonopolyResponses> hubContext)
    : MonopolyEventHandlerBase<PlayerRedeemEvent>
{
    protected override Task HandleSpecificEvent(PlayerRedeemEvent e)
    {
        return hubContext.Clients.All.PlayerRedeemEvent(new PlayerRedeemEventArgs
        {
            PlayerId = e.PlayerId,
            PlayerMoney = e.PlayerMoney,
            LandId = e.LandId,
        });
    }
}