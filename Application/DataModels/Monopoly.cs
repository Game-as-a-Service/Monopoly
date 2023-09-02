namespace Application.DataModels;

public record Monopoly(string Id, Player[] Players, Map Map, Player Host, Player CurrentPlayer);

public record Player(string Id, decimal Money, Chess Chess);
public record Chess(string CurrentPosition, Direction Direction, int RemainSteps);
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