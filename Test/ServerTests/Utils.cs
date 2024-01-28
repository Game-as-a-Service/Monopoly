using Application.Common;
using Application.DataModels;
using Domain;
using Domain.Interfaces;
using ServerTests.Usecases;
using SharedLibrary;
using SharedLibrary.ResponseArgs.Monopoly;

namespace ServerTests;

public class Utils
{
    //public static IDice[]? MockDice(params int[] diceValues)
    //{
    //    var dice = new IDice[diceValues.Length];
    //    for (int i = 0; i < diceValues.Length; i++)
    //    {
    //        var mockDice = new Mock<IDice>();
    //        mockDice.Setup(x => x.Roll());
    //        mockDice.Setup(x => x.Value).Returns(diceValues[i]);
    //        dice[i] = mockDice.Object;
    //    }

    //    return dice;
    //}

    internal static void VerifyChessMovedEvent(VerificationHub hub, string playerId, string blockId, string direction, int remainingSteps)
    {
        hub.Verify(nameof(IMonopolyResponses.ChessMovedEvent), (ChessMovedEventArgs e) =>
            e.PlayerId == playerId && e.BlockId == blockId && e.Direction == direction && e.RemainingSteps == remainingSteps);
    }

    public class MonopolyBuilder
    {
        public string GameId { get; private set; }

        public List<Application.DataModels.Player> Players { get; private set; } = new();

        public string HostId { get; private set; }

        public int[] Dices { get; private set; } = { 0 };

        public Application.DataModels.CurrentPlayerState CurrentPlayerState { get; private set; }
        public List<LandHouse> LandHouses { get; private set; } = new();
        public Application.DataModels.Map Map { get; private set; }
        public Application.DataModels.GameStage GameStage { get; private set; }

        public MonopolyBuilder(string id)
        {
            GameId = id;
            GameStage = Application.DataModels.GameStage.Gaming;
        }

        public MonopolyBuilder WithPlayer(Application.DataModels.Player player)
        {
            Players.Add(player);
            return this;
        }

        public MonopolyBuilder WithMockDice(int[] dices)
        {
            Dices = dices;
            return this;
        }

        public MonopolyBuilder WithCurrentPlayer(Application.DataModels.CurrentPlayerState currentPlayerState)
        {
            CurrentPlayerState = currentPlayerState;
            return this;
        }

        public MonopolyBuilder WithHost(string id)
        {
            HostId = id;
            return this;
        }

        public MonopolyBuilder WithLandHouse(string id, int house)
        {
            LandHouses.Add(new LandHouse(id, house));
            return this;
        }

        public Application.DataModels.Monopoly Build()
        {
            return new Application.DataModels.Monopoly(Id: GameId,
                                Players: Players.ToArray(),
                                Map: Map,
                                HostId: HostId,
                                GameStage: GameStage,
                                CurrentPlayerState: CurrentPlayerState,
                                LandHouses: LandHouses.ToArray());
        }

        internal void Save(MonopolyTestServer server)
        {
            var monopoly = Build();
            server.GetRequiredService<ICommandRepository>().Save(monopoly);
            server.GetRequiredService<MockDiceService>().Dices = Dices.Select(value => new MockDice(value)).ToArray<IDice>();
        }

        internal MonopolyBuilder WithGameStage(Application.DataModels.GameStage gameStage)
        {
            GameStage = gameStage;
            return this;
        }
    }

    public class PlayerBuilder
    {
        public string Id { get; set; }
        public decimal Money { get; set; }
        public string BlockId { get; set; }
        public Direction Direction { get; set; }
        public List<Application.DataModels.LandContract> LandContracts { get; set; }
        public PlayerState PlayerState { get; set; }
        public int BankruptRounds { get; set; }
        public string RoleId { get; set; }
        public int LocationId { get; set; }

        public PlayerBuilder(string id)
        {
            Id = id;
            Money = 15000;
            BlockId = "StartPoint";
            Direction = Direction.Right;
            LandContracts = new();
            PlayerState = PlayerState.Normal;
        }

        public PlayerBuilder WithMoney(decimal money)
        {
            Money = money;
            return this;
        }

        public PlayerBuilder WithPosition(string blockId, Direction direction)
        {
            BlockId = blockId;
            Direction = direction;
            return this;
        }

        public PlayerBuilder WithLandContract(string LandId, bool InMortgage = false, int Deadline = 10)
        {
            LandContracts.Add(new Application.DataModels.LandContract(LandId: LandId,
                                               InMortgage: InMortgage,
                                               Deadline: Deadline));
            return this;
        }

        public PlayerBuilder WithBankrupt(int rounds)
        {
            PlayerState = PlayerState.Bankrupt;
            BankruptRounds = rounds;
            return this;
        }

        public Application.DataModels.Player Build()
        {
            Application.DataModels.Chess chess = new(CurrentPosition: BlockId,
                              Direction: Enum.Parse<Application.DataModels.Direction>(Direction.ToString()));
            Application.DataModels.Player player = new(Id: Id,
                                    Money: Money,
                                    Chess: chess,
                                    LandContracts: LandContracts.ToArray(),
                                    PlayerState: PlayerState,
                                    BankruptRounds,
                                    LocationId: LocationId,
                                    RoleId: RoleId
                                    );
            return player;
        }

        internal PlayerBuilder WithRole(string roleId)
        {
            RoleId = roleId;
            return this;
        }

        internal PlayerBuilder WithLocation(int locationId)
        {
            LocationId = locationId;
            return this;
        }

        internal PlayerBuilder WithState(PlayerState playerState)
        {
            PlayerState = playerState;
            return this;
        }
    }

    public enum Direction
    {
        Up,
        Down,
        Left,
        Right
    }

    public class CurrentPlayerStateBuilder
    {
        public string Id { get; private set; }
        public bool IsPayToll { get; private set; }
        public bool IsBoughtLand { get; private set; }
        public bool IsUpgradeLand { get; private set; }
        public Application.DataModels.Auction? Auction { get; private set; }
        public int RemainingSteps { get; private set; }
        public bool HadSelectedDirection { get; private set; }

        public CurrentPlayerStateBuilder(string id)
        {
            Id = id;
            IsPayToll = false;
            IsBoughtLand = false;
            IsUpgradeLand = false;
            RemainingSteps = 0;
            HadSelectedDirection = false;
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

        internal CurrentPlayerStateBuilder WithAuction(string LandId, string HighestBidderId, decimal HighestPrice)
        {
            Auction = new Application.DataModels.Auction(LandId, HighestBidderId, HighestPrice);
            return this;
        }

        internal CurrentPlayerStateBuilder WithRemainingSteps(int remainingSteps)
        {
            RemainingSteps = remainingSteps;
            return this;
        }

        internal CurrentPlayerStateBuilder HadNotSelectedDirection()
        {
            HadSelectedDirection = false;
            return this;
        }

        public Application.DataModels.CurrentPlayerState Build()
        {
            return new Application.DataModels.CurrentPlayerState(PlayerId: Id,
                                          IsPayToll: IsPayToll,
                                          IsBoughtLand: IsBoughtLand,
                                          IsUpgradeLand: IsUpgradeLand,
                                          Auction: Auction,
                                          RemainingSteps: RemainingSteps,
                                          HadSelectedDirection: HadSelectedDirection);
        }
    }
}