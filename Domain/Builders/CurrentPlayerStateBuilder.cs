namespace Domain.Builders;

public class CurrentPlayerStateBuilder
{
    public Player Player { get; private set; }
    public bool IsPayToll { get; private set; }
    public bool IsBoughtLand { get; private set; }
    public bool IsUpgradeLand { get; private set; }
    public Auction? Auction { get; private set; }
    public CurrentPlayerStateBuilder(Player player)
    {
        Player = player;
        IsPayToll = false;
        IsBoughtLand = false;
        IsUpgradeLand = false;
    }

    public CurrentPlayerStateBuilder WithPayToll()
    {
        IsPayToll = true;
        return this;
    }

    public CurrentPlayerStateBuilder WithBoughtLand()
    {
        IsBoughtLand = true;
        return this;
    }

    public CurrentPlayerStateBuilder WithUpgradeLand()
    {
        IsUpgradeLand = true;
        return this;
    }

    internal CurrentPlayerStateBuilder WithAuction(LandContract landContract, Player? HighestBidder = null, decimal? HighestPrice = null)
    {
        if (HighestBidder is null) 
        {
            Auction = new Auction(landContract);
        }
        else
        {
            Auction = new Auction(landContract, HighestBidder, (decimal)HighestPrice!);
        }
        return this;
    }

    public CurrentPlayerState Build()
    {
        return new CurrentPlayerState(PlayerId: Player.Id,
                                      IsPayToll: IsPayToll,
                                      IsBoughtLand: IsBoughtLand,
                                      IsUpgradeLand: IsUpgradeLand,
                                      Auction: Auction);
    }
}