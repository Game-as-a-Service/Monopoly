using Domain.Exceptions;
using Domain.Events;
using Domain.Common;

namespace Domain;

public class Auction
{
    private readonly LandContract landContract;
    private Player? highestBidder;
    private decimal highestPrice;

    public Auction(LandContract landContract)
    {
        this.landContract = landContract;
        highestPrice = landContract.Land.GetPrice("Auction");
    }

    public Auction(LandContract landContract, Player highestBidder, decimal highestPrice)
    {
        this.landContract = landContract;
        this.highestBidder = highestBidder;
        this.highestPrice = highestPrice;
    }

    /// <summary>
    /// 結算拍賣
    /// 將地契轉移給最高出價者
    /// 若流拍，將原有地契持有者的地契移除
    ///
    /// 流拍金額為原土地價值的 70%
    /// </summary>
    internal DomainEvent End()
    {
        landContract.Owner.RemoveLandContract(landContract);
        if (highestBidder != null)
        {
            highestBidder.AddLandContract(new LandContract(highestBidder, landContract.Land));
            highestBidder.Money -= highestPrice;

            landContract.Owner.Money += highestPrice;
            landContract.Land.UpdateOwner(highestBidder);
        }
        else // 流拍
        {
            landContract.Owner.Money += landContract.Land.GetPrice("UnSold");
            landContract.Land.UpdateOwner(null);
        }
        return new EndAuctionEvent(landContract.Owner.Monopoly.Id, 
                                   landContract.Owner.Id, 
                                   landContract.Owner.Money, 
                                   landContract.Land.Id, 
                                   highestBidder?.Id,  
                                   highestBidder == null ? 0 : highestBidder.Money);
    }

    internal DomainEvent Bid(Player player, decimal price)
    {
        if (price < highestPrice || (price == highestPrice && highestBidder is not null))
        {
            return new PlayerBidFailEvent(player.Monopoly.Id, player.Id, landContract.Land.Id, price, highestPrice);
            //throw new BidException($"出價要大於{highestPrice}");
        }
        else if (price > player.Money)
        {
            
            return new PlayerTooPoorToBidEvent(player.Monopoly.Id, player.Id, player.Money, price, highestPrice);
            //throw new BidException($"現金少於{price}");
        }
        highestBidder = player;
        highestPrice = price;
        return new PlayerBidEvent(player.Monopoly.Id, player.Id, landContract.Land.Id, highestPrice);
        
    }
}