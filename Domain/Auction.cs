﻿using Domain.Exceptions;
using Domain.Events;
using Domain.Common;

namespace Domain;

public class Auction
{
    private LandContract landContract;
    private Player? highestBidder;
    private decimal highestPrice;

    public Auction(LandContract landContract)
    {
        this.landContract = landContract;
        highestPrice = landContract.Land.Price * (decimal)0.5;
    }

    /// <summary>
    /// 結算拍賣
    /// 將地契轉移給最高出價者
    /// 若流拍，將原有地契持有者的地契移除
    ///
    /// 流拍金額為原土地價值的 70%
    /// </summary>
    internal void End()
    {
        landContract.Owner.RemoveLandContract(landContract);
        if (highestBidder != null)
        {
            highestBidder.AddLandContract(landContract with { Owner = highestBidder });
            highestBidder.Money -= highestPrice;

            landContract.Owner.Money += highestPrice;
        }
        else // 流拍
        {
            landContract.Owner.Money += landContract.Land.Price * (decimal)0.7;
        }
    }

    internal DomainEvent Bid(Player player, decimal price)
    {
        if (price <= highestPrice)
        {
            return new PlayerBidFailEvent(player.Monopoly.Id, player.Id, landContract.Land.Id, highestPrice);
            //throw new BidException($"出價要大於{highestPrice}");
        }
        else if (price > player.Money)
        {
            
            return new PlayerTooPoorToBidEvent(player.Monopoly.Id, player.Id, player.Money, highestPrice);
            //throw new BidException($"現金少於{price}");
        }
        highestBidder = player;
        highestPrice = price;
        return new PlayerBidEvent(player.Monopoly.Id, player.Id, landContract.Land.Id, highestPrice);
        
    }
}