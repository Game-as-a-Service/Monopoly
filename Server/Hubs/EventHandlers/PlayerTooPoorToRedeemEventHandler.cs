using Domain.Events;
using Microsoft.AspNetCore.SignalR;
using Server.Common;
using SharedLibrary;
using SharedLibrary.ResponseArgs.Monopoly;

namespace Server.Hubs.EventHandlers;

public class PlayerTooPoorToRedeemEventHandler(IHubContext<MonopolyHub, IMonopolyResponses> hubContext)
    : MonopolyEventHandlerBase<PlayerTooPoorToRedeemEvent>
{
    protected override Task HandleSpecificEvent(PlayerTooPoorToRedeemEvent e)
    {
        return hubContext.Clients.All.PlayerTooPoorToRedeemEvent(new PlayerTooPoorToRedeemEventArgs
        {
            PlayerId = e.PlayerId,
            PlayerMoney = e.PlayerMoney,
            LandId = e.BlockId,
            RedeemPrice = e.RedeemPrice,
        });
    }
}