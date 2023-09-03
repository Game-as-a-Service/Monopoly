namespace Application.DataModels;

public record Monopoly(string Id, Player[] Players, Map Map, string HostId, CurrentPlayerState CurrentPlayerState);

public record Player(string Id, decimal Money, Chess Chess, LandContract[] LandContracts);
public record CurrentPlayerState(string PlayerId, bool IsPayToll, bool IsBoughtLand, bool IsUpgradeLand, Auction? Auction);
public record Chess(string CurrentPosition, Direction Direction, int RemainSteps);
public record LandContract(string LandId, bool InMortgage, int Deadline);
public record Auction(string LandId, string? HighestBidderId = null, decimal? HighestPrice = null);
public enum Direction
{
    Up,
    Down,
    Left,
    Right
}

public record Map(string Id, Block?[][] Blocks);

public abstract record Block(string Id);

public record EmptyBlock() : Block(string.Empty);

public record Land(string Id) : Block(Id);

public record ParkingLot(string Id) : Block(Id);

public record Jail(string Id) : Block(Id);

public record StartPoint(string Id) : Block(Id);

public record Station(string Id) : Land(Id);