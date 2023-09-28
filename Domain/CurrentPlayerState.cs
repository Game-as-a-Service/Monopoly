namespace Domain;

public record CurrentPlayerState(string PlayerId,
                                 bool IsPayToll = false,
                                 bool IsBoughtLand = false,
                                 bool IsUpgradeLand = false,
                                 Auction? Auction = null,
                                 int RemainingSteps = 0)
{
    public bool CanEndRound => IsPayToll;
}
