namespace Domain;

public record CurrentPlayerState(string PlayerId, bool IsPayToll = false, bool IsBoughtLand = false, bool IsUpgradeLand = false, Auction? Auction = null);
