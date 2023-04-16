using Application.Common;
using Domain.Common;
using Domain.Events;
using Microsoft.AspNetCore.SignalR;
using Server.Hubs;

namespace Server;

public class MonopolyEventBus : IEventBus<DomainEvent>
{
    private readonly IHubContext<MonopolyHub, IMonopolyResponses> _hubContext;

    public MonopolyEventBus(IHubContext<MonopolyHub, IMonopolyResponses> hubContext)
    {
        _hubContext = hubContext;
    }
    // 這裡暫時採用
    // 1. 不同的event，使用不同的發送方式
    // 2. 傳送到所有使用者
    // 3. 之後要視不同的Event發送到不同的Client。
    //    像是個人的錯誤，只傳送到特定使用者
    //    像是遊戲的狀態，傳送到Group Id為Game Id的所有玩家
    public async Task PublishAsync(IEnumerable<DomainEvent> events)
    {
        foreach (var e in events)
        {
            if (e is GameCreatedEvent gce) 
            {
                await _hubContext.Clients.All.GameCreatedEvent(gce.GameId);
            }
            else if (e is PlayerRolledDiceEvent prde)
            {
                await _hubContext.Clients.All.PlayerRolledDiceEvent(prde.PlayerId, prde.DiceCount);
            }
            else if (e is ChessMovedEvent cme)
            {
                await _hubContext.Clients.All.ChessMovedEvent(cme.PlayerId, cme.BlockId, cme.Direction, cme.RemainingSteps);
            }
            else if (e is PlayerNeedToChooseDirectionEvent pnsde)
            {
                await _hubContext.Clients.All.PlayerNeedToChooseDirectionEvent(pnsde.PlayerId, pnsde.Directions);
            }
            else if (e is ThroughStartEvent tse)
            {
                await _hubContext.Clients.All.ThroughStartEvent(tse.PlayerId, tse.GainMoney, tse.TotalMoney);
            }
        }
    }
}