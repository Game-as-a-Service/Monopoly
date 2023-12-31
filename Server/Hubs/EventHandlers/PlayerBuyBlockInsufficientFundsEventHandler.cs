using Domain.Events;
using Microsoft.AspNetCore.SignalR;
using Server.Common;
using SharedLibrary;
using SharedLibrary.ResponseArgs.Monopoly;

namespace Server.Hubs.EventHandlers;

public class PlayerBuyBlockInsufficientFundsEventHandler(IHubContext<MonopolyHub, IMonopolyResponses> hubContext)
    : MonopolyEventHandlerBase<PlayerBuyBlockInsufficientFundsEvent>
{
    protected override Task HandleSpecificEvent(PlayerBuyBlockInsufficientFundsEvent e)
    {
        return hubContext.Clients.All.PlayerBuyBlockInsufficientFundsEvent(
            new PlayerBuyBlockInsufficientFundsEventArgs
            {
                PlayerId = e.PlayerId,
                LandId = e.LandId,
                Price = e.Price,
            }
        );
    }
}