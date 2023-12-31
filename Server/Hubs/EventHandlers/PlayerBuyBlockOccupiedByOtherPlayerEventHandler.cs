using Domain.Events;
using Microsoft.AspNetCore.SignalR;
using Server.Common;
using SharedLibrary;
using SharedLibrary.ResponseArgs.Monopoly;

namespace Server.Hubs.EventHandlers;

public class PlayerBuyBlockOccupiedByOtherPlayerEventHandler(IHubContext<MonopolyHub, IMonopolyResponses> hubContext)
    : MonopolyEventHandlerBase<PlayerBuyBlockOccupiedByOtherPlayerEvent>
{
    protected override Task HandleSpecificEvent(PlayerBuyBlockOccupiedByOtherPlayerEvent e)
    {
        return hubContext.Clients.All.PlayerBuyBlockOccupiedByOtherPlayerEvent(
            new PlayerBuyBlockOccupiedByOtherPlayerEventArgs
            {
                PlayerId = e.PlayerId,
                LandId = e.LandId,
            }
        );
    }
}