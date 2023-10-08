using Application.DataModels;
using Domain.Maps;

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
    /// <summary>
    /// (Monopoly) Domain to Application
    /// </summary>
    /// <param name="domainMonopoly"></param>
    /// <returns></returns>
    private static Monopoly ToApplication(this Domain.Monopoly domainMonopoly)
    {
        Player[] players = domainMonopoly.Players.Select(player =>
        {
            var playerChess = player.Chess;

            Chess chess = new(playerChess.CurrentBlockId, playerChess.CurrentDirection.ToApplicationDirection(), playerChess.RemainingSteps);

            var landContracts = player.LandContractList.Select(contract =>
            new LandContract(contract.Land.Id, contract.InMortgage, contract.Deadline)).ToArray();

            return new Player(
                        player.Id,
                        player.Money,
                        chess,
                        landContracts,
                        player.IsBankrupt,
                        player.BankruptRounds);
        }).ToArray();

        Map map = new(domainMonopoly.Map.Id, domainMonopoly.Map.Blocks
            .Select(row =>
            {
                return row.Select(block => block?.ToApplicationBlock()).ToArray();
            }).ToArray()
        );
        var currentPlayer = domainMonopoly.Players.First(player => player.Id == domainMonopoly.CurrentPlayerState.PlayerId);
        var auction = domainMonopoly.CurrentPlayerState.Auction;
        var currentPlayerState = new CurrentPlayerState(
            domainMonopoly.CurrentPlayerState.PlayerId,
            domainMonopoly.CurrentPlayerState.IsPayToll,
            domainMonopoly.CurrentPlayerState.IsBoughtLand,
            domainMonopoly.CurrentPlayerState.IsUpgradeLand,
            domainMonopoly.CurrentPlayerState.Auction is null ? null : new Auction(auction.LandContract.Land.Id, auction.HighestBidder?.Id, auction.HighestPrice)
            );
        var LandHouses = domainMonopoly.Map.Blocks.SelectMany(block => block).OfType<Domain.Land>()
                                                  .Where(land => land.House > 0)
                                                  .Select(land => new LandHouse(land.Id, land.House)).ToArray();

        return new Monopoly(domainMonopoly.Id, players, map, domainMonopoly.HostId, currentPlayerState, LandHouses);
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
    /// <summary>
    /// (Monopoly) Application to Domain
    /// </summary>
    /// <param name="monopoly"></param>
    /// <returns></returns>
    internal static Domain.Monopoly ToDomain(this Monopoly monopoly)
    {
        //Domain.Map map = new(monopoly.Map.Id, monopoly.Map.Blocks
        //               .Select(row =>
        //               {
        //                   return row.Select(block => block?.ToDomainBlock()).ToArray();
        //               }).ToArray()
        //           );
        Domain.Map map = new SevenXSevenMap();
        var builder = new Domain.Builders.MonopolyBuilder()
            .WithId(monopoly.Id)
            .WithHost(monopoly.HostId)
            .WithMap(map);
        monopoly.Players.ToList().ForEach(
            p => builder.WithPlayer(p.Id, playerBuilder =>
                playerBuilder.WithMoney(p.Money)
                     .WithPosition(p.Chess.CurrentPosition, p.Chess.Direction.ToString())
                     .WithLandContracts(p.LandContracts)
                     .WithBankrupt(p.IsBankrupt, p.BankruptRounds)
            ));
        var cps = monopoly.CurrentPlayerState;
        if (cps.Auction is null)
        {
            builder.WithCurrentPlayer(cps.PlayerId, x => x.WithBoughtLand(cps.IsBoughtLand)
                                                          .WithUpgradeLand(cps.IsUpgradeLand)
                                                          .WithPayToll(cps.IsPayToll));
        }
        else
        {
            builder.WithCurrentPlayer(cps.PlayerId, x => x.WithAuction(
                cps.Auction.LandId, cps.Auction.HighestBidderId, cps.Auction.HighestPrice));
        }
        monopoly.LandHouses.ToList().ForEach(LandHouse => builder.WithLandHouse(LandHouse.LandId, LandHouse.House));

        return builder.Build();
    }
    private static Domain.Builders.PlayerBuilder WithLandContracts(this Domain.Builders.PlayerBuilder builder, LandContract[] landContracts)
    {
        landContracts.ToList().ForEach(landContract =>
        {
            builder.WithLandContract(landContract.LandId, landContract.InMortgage, landContract.Deadline);
        });
        return builder;
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