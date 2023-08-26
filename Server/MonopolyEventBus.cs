using Application.Common;
using Domain.Common;
using Domain.Events;
using Microsoft.AspNetCore.SignalR;
using Server.Hubs;
using SharedLibrary;

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
            else if (e is OnStartEvent ose)
            {
                await _hubContext.Clients.All.OnStartEvent(ose.PlayerId, ose.GainMoney, ose.TotalMoney);
            }
            else if (e is PlayerCanBuildHouseEvent cbhe)
            {
                await _hubContext.Clients.All.PlayerCanBuildHouseEvent(cbhe.PlayerId, cbhe.BlockId, cbhe.HouseCount, cbhe.UpgradeMoney);
            }
            else if (e is PlayerCanBuyLandEvent cble)
            {
                await _hubContext.Clients.All.PlayerCanBuyLandEvent(cble.PlayerId, cble.BlockId, cble.landMoney);
            }
            else if (e is PlayerChooseDirectionEvent pcde)
            {
                await _hubContext.Clients.All.PlayerChooseDirectionEvent(pcde.PlayerId, pcde.Direction);
            }
            else if (e is PlayerCannotMoveEvent pcme)
            {
                await _hubContext.Clients.All.PlayerCannotMoveEvent(pcme.PlayerId, pcme.SuspendRounds);
            }
            else if (e is PlayerNeedsToPayTollEvent pntpte)
            {
                await _hubContext.Clients.All.PlayerNeedsToPayTollEvent(pntpte.PlayerId, pntpte.ownerId, pntpte.toll);
            }
            else if (e is PlayerBuyBlockEvent pbbe)
            {
                await _hubContext.Clients.All.PlayerBuyBlockEvent(pbbe.PlayerId, pbbe.BlockId);
            }
            else if (e is PlayerBuyBlockMissedLandEvent pbbmle)
            {
                await _hubContext.Clients.All.PlayerBuyBlockMissedLandEvent(pbbmle.PlayerId, pbbmle.BlockId);
            }
            else if (e is PlayerBuyBlockOccupiedByOtherPlayerEvent pbbobope)
            {
                await _hubContext.Clients.All.PlayerBuyBlockOccupiedByOtherPlayerEvent(pbbobope.PlayerId, pbbobope.BlockId);
            }
            else if (e is PlayerBuyBlockInsufficientFundsEvent pbbife)
            {
                await _hubContext.Clients.All.PlayerBuyBlockInsufficientFundsEvent(pbbife.PlayerId, pbbife.BlockId, pbbife.landMoney);
            }
            else if (e is PlayerPayTollEvent ppte)
            {
                await _hubContext.Clients.All.PlayerPayTollEvent(ppte.PlayerId, ppte.PlayerMoney, ppte.ownerId, ppte.ownerMoney);
            }
            else if (e is PlayerDoesntNeedToPayTollEvent pdnpte)
            {
                await _hubContext.Clients.All.PlayerDoesntNeedToPayTollEvent(pdnpte.PlayerId, pdnpte.PlayerMoney);
            }
            else if (e is PlayerTooPoorToPayTollEvent ptppte)
            {
                await _hubContext.Clients.All.PlayerTooPoorToPayTollEvent(ptppte.PlayerId, ptppte.PlayerMoney, ptppte.toll);
            }
            else if (e is PlayerBuildHouseEvent pbhe)
            {
                await _hubContext.Clients.All.PlayerBuildHouseEvent(pbhe.PlayerId, pbhe.BlockId, pbhe.PlayerMoney, pbhe.House);
            }
            else if (e is PlayerCannotBuildHouseEvent pcbhe)
            {
                await _hubContext.Clients.All.PlayerCannotBuildHouseEvent(pcbhe.PlayerId, pcbhe.BlockId);
            }
            else if (e is PlayerTooPoorToBuildHouseEvent ptpbhe)
            {
                await _hubContext.Clients.All.PlayerTooPoorToBuildHouseEvent(ptpbhe.PlayerId, ptpbhe.BlockId, ptpbhe.PlayerMoney, ptpbhe.UpgradePrice);
            }
            else if (e is HouseMaxEvent hme)
            {
                await _hubContext.Clients.All.HouseMaxEvent(hme.PlayerId, hme.BlockId, hme.House);
            }
            else if (e is PlayerMortgageEvent pme)
            {
                await _hubContext.Clients.All.PlayerMortgageEvent(pme.PlayerId, pme.PlayerMoney, pme.BlockId, pme.DeadLine);
            }
            else if (e is PlayerCannotMortgageEvent pctme)
            {
                await _hubContext.Clients.All.PlayerCannotMortgageEvent(pctme.PlayerId, pctme.PlayerMoney, pctme.BlockId);
            }
            else if (e is MorgageDueEvent mde)
            {
                await _hubContext.Clients.All.MortgageDueEvent(mde.PlayerId, mde.BlockId);
            }
            else if (e is MorgageCountdownEvent mce)
            {
                await _hubContext.Clients.All.MortgageCountdownEvent(mce.PlayerId, mce.BlockId, mce.DeadLine);
            }
            else if (e is PlayerRedeemEvent pre)
            {
                await _hubContext.Clients.All.PlayerRedeemEvent(pre.PlayerId, pre.PlayerMoney, pre.BlockId);
            }
            else if (e is PlayerTooPoorToRedeemEvent ptpre)
            {
                await _hubContext.Clients.All.PlayerTooPoorToRedeemEvent(ptpre.PlayerId, ptpre.PlayerMoney, ptpre.BlockId, ptpre.RedeemPrice);
            }
            else if (e is LandNotInMortgageEvent lnime)
            {
                await _hubContext.Clients.All.LandNotInMortgageEvent(lnime.PlayerId, lnime.BlockId);
            }
            else if (e is PlayerBidEvent pbe)
            {
                await _hubContext.Clients.All.PlayerBidEvent(pbe.PlayerId, pbe.BlockId, pbe.HighestPrice);
            }
            else if (e is PlayerBidFailEvent pbfe)
            {
                await _hubContext.Clients.All.PlayerBidFailEvent(pbfe.PlayerId, pbfe.BlockId, pbfe.BidPrice, pbfe.HighestPrice);
            }
            else if (e is PlayerTooPoorToBidEvent ptpbe)
            {
                await _hubContext.Clients.All.PlayerTooPoorToBidEvent(ptpbe.PlayerId, ptpbe.PlayerMoney, ptpbe.BidPrice, ptpbe.HighestPrice);
            }
            else if (e is CurrentPlayerCannotBidEvent cpcbe)
            {
                await _hubContext.Clients.All.CurrentPlayerCannotBidEvent(cpcbe.PlayerId);
            }
            else if (e is EndAuctionEvent eae)
            {
                await _hubContext.Clients.All.EndAuctionEvent(eae.PlayerId, eae.PlayerMoney, eae.BlockId, eae.Owner, eae.OwnerMoney);
            }
            else if (e is EndRoundEvent ere)
            {
                await _hubContext.Clients.All.EndRoundEvent(ere.PlayerId, ere.NextPlayerId);
            }
            else if (e is EndRoundFailEvent erfe)
            {
                await _hubContext.Clients.All.EndRoundFailEvent(erfe.PlayerId);
            }
            else if (e is SuspendRoundEvent sre)
            {
                await _hubContext.Clients.All.SuspendRoundEvent(sre.PlayerId, sre.SuspendRounds);
            }
            else if (e is BankruptEvent be)
            {
                await _hubContext.Clients.All.BankruptEvent(be.PlayerId);
            }
            else if (e is SettlementEvent se)
            {
                await _hubContext.Clients.All.SettlementEvent(se.PlayerId, se.Rank);
            }
        }
    }
}