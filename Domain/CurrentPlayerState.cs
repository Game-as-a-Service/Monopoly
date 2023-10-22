namespace Domain;

public record CurrentPlayerState(string PlayerId,
                                 bool IsPayToll,
                                 bool IsBoughtLand,
                                 bool IsUpgradeLand,
                                 Auction? Auction,
                                 int RemainingSteps,
                                 bool HadSelectedDirection)
{
    public bool CanEndRound => IsPayToll;
}
