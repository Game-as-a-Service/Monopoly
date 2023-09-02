using Application.DataModels;

namespace Application.Common;

public interface IRepository
{
    public Monopoly FindGameById(string id);
    public string[] GetRooms();
    public bool IsExist(string id);
    public string Save(Monopoly monopoly);
}

internal static class RepositoryExtensions
{
    internal static string Save(this IRepository repository, Domain.Monopoly domainMonopoly)
    {
        Monopoly monopoly = domainMonopoly.ToApplication();
        return repository.Save(monopoly);
    }
    private static Monopoly ToApplication(this Domain.Monopoly domainMonopoly)
    {
        Player[] players = domainMonopoly.Players.Select(player => new Player(
            player.Id, 
            player.Money, 
            new Chess(player.Chess.CurrentBlock.Id, player.Chess.CurrentDirection.ToApplicationDirection(), player.Chess.RemainingSteps)
            )).ToArray();

        Map map = new(domainMonopoly.Map.Id, domainMonopoly.Map.Blocks
            .Select(row =>
            {
                return row.Select(block => block?.ToApplicationBlock()).ToArray();
            }).ToArray()
        );
        var currentPlayer = players.First(p => p.Id == domainMonopoly.CurrentPlayer.Id);
        var host = players.First(p => p.Id == domainMonopoly.Host);
        return new Monopoly(domainMonopoly.Id, players, map, host, currentPlayer);
    }
    private static Block ToApplicationBlock(this Domain.Block domainBlock)
    {
        return domainBlock switch
        {
            Domain.StartPoint startBlock => new StartPoint(startBlock.Id),
            Domain.Station stationBlock => new Station(stationBlock.Id),
            Domain.Land landBlock => new Land(landBlock.Id),
            Domain.ParkingLot parkingLotBlock => new ParkingLot(parkingLotBlock.Id),
            Domain.Jail prisonBlock => new Jail(prisonBlock.Id),
            null => new EmptyBlock(),
            _ => throw new NotImplementedException(),
        };
    }
    internal static Domain.Monopoly ToDomain(this Monopoly monopoly)
    {
        Domain.Player[] players = monopoly.Players.Select(player => new Domain.Player(player.Id)).ToArray();
        Domain.Map map = new(monopoly.Map.Id, monopoly.Map.Blocks
                       .Select(row =>
                       {
                           return row.Select(block => block?.ToDomainBlock()).ToArray();
                       }).ToArray()
                   );
        var domainMonopoly = new Domain.Monopoly(monopoly.Id, map);
        foreach (var player in players)
        {
            domainMonopoly.AddPlayer(player);
        }
        return domainMonopoly;
    }
    private static Domain.Block? ToDomainBlock(this Block? block)
    {
        return block switch
        {
            StartPoint startBlock => new Domain.StartPoint(startBlock.Id),
            Station stationBlock => new Domain.Station(stationBlock.Id),
            Land landBlock => new Domain.Land(landBlock.Id),
            ParkingLot parkingLotBlock => new Domain.ParkingLot(parkingLotBlock.Id),
            Jail prisonBlock => new Domain.Jail(prisonBlock.Id),
            EmptyBlock => null,
            _ => throw new NotImplementedException(),
        };
    }
    private static Direction ToApplicationDirection(this Domain.Map.Direction direction)
    {
        return direction switch
        {
            Domain.Map.Direction.Up => Direction.Up,
            Domain.Map.Direction.Down => Direction.Down,
            Domain.Map.Direction.Left => Direction.Left,
            Domain.Map.Direction.Right => Direction.Right,
            _ => throw new NotImplementedException(),
        };
    }
}