namespace Domain.Builders;

public class CurrentPlayerStateBuilder
{
    public string PlayerId { get; private set; }
    public bool IsPayToll { get; private set; }
    public bool IsBoughtLand { get; private set; }
    public bool IsUpgradeLand { get; private set; }
    public bool HasAuction { get; private set; }
    public (string LandId, string? HighestBidder, decimal HighestPrice) Auction { get; private set; }
    public int RemainingSteps { get; private set; }
    public CurrentPlayerStateBuilder(string Id)
    {
        PlayerId = Id;
        IsPayToll = false;
        IsBoughtLand = false;
        IsUpgradeLand = false;
        HasAuction = false;
    }

    public CurrentPlayerStateBuilder WithPayToll(bool isPayToll = true)
    {
        IsPayToll = isPayToll;
        return this;
    }

    public CurrentPlayerStateBuilder WithBoughtLand(bool isBoughtLand = true)
    {
        IsBoughtLand = isBoughtLand;
        return this;
    }

    public CurrentPlayerStateBuilder WithUpgradeLand(bool isUpgradeLand = true)
    {
        IsUpgradeLand = isUpgradeLand;
        return this;
    }

    public CurrentPlayerStateBuilder WithAuction(string landId, string HighestBidder, decimal HighestPrice)
    {
        HasAuction = true;
        Auction = (landId, HighestBidder, HighestPrice);
        return this;
    }

    internal CurrentPlayerState Build(Auction? auction)
    {
        return new CurrentPlayerState(PlayerId: PlayerId,
                                      IsPayToll: IsPayToll,
                                      IsBoughtLand: IsBoughtLand,
                                      IsUpgradeLand: IsUpgradeLand,
                                      Auction: auction);
    }

    
}