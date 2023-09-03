using Application.Common;
using Application.DataModels;
using Moq;
using SharedLibrary;

namespace ServerTests;

public class Utils
{
    public static IDice[]? MockDice(params int[] diceValues)
    {
        var dice = new IDice[diceValues.Length];
        for (int i = 0; i < diceValues.Length; i++)
        {
            var mockDice = new Mock<IDice>();
            mockDice.Setup(x => x.Roll());
            mockDice.Setup(x => x.Value).Returns(diceValues[i]);
            dice[i] = mockDice.Object;
        }

        return dice;
    }

    internal static void VerifyChessMovedEvent(VerificationHub hub, string playerId, string blockId, string direction, int remainingSteps)
    {
        hub.Verify<string, string, string, int>(nameof(IMonopolyResponses.ChessMovedEvent), (PlayerId, BlockId, Direction, RemainingSteps) =>
            PlayerId == playerId && BlockId == blockId && Direction == direction && RemainingSteps == remainingSteps);
    }

    public class MonopolyBuilder
    {
        public string GameId { get; private set; }

        public List<Player> Players { get; private set; } = new();

        public string HostId { get; private set; }

        public int[] Dices { get; private set; }

        public CurrentPlayerState CurrentPlayerState { get; private set; }
        public Map Map { get; private set; }

        public MonopolyBuilder(string id)
        {
            GameId = id;
        }

        public MonopolyBuilder WithPlayer(Player player)
        {
            Players.Add(player);
            return this;
        }

        public MonopolyBuilder WithMockDice(int[] dices)
        {
            Dices = dices;
            return this;
        }

        public MonopolyBuilder WithCurrentPlayer(CurrentPlayerState currentPlayerState)
        {
            CurrentPlayerState = currentPlayerState;
            return this;
        }

        public MonopolyBuilder WithHost(string id)
        {
            HostId = id;
            return this;
        }

        public Monopoly Build()
        {
            return new Monopoly(Id: GameId,
                                Players: Players.ToArray(),
                                Map: Map,
                                HostId: HostId,
                                CurrentPlayerState: CurrentPlayerState);
        }

        internal void Save(MonopolyTestServer server)
        {
            var monopoly = Build();
            server.GetRequiredService<IRepository>().Save(monopoly);
        }
    }

    public class PlayerBuilder
    {
        public string Id { get; set; }
        public decimal Money { get; set; }
        public string BlockId { get; set; }
        public Direction Direction { get; set; }
        public List<LandContract> LandContracts { get; set; }
        public bool Bankrupt { get; set; }

        public PlayerBuilder(string id)
        {
            Id = id;
            Money = 15000;
            BlockId = "StartPoint";
            Direction = Direction.Right;
            LandContracts = new();
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
            LandContracts.Add(new LandContract(LandId: LandId,
                                               InMortgage: InMortgage,
                                               Deadline: Deadline));
            return this;
        }

        public PlayerBuilder WithBankrupt()
        {
            Bankrupt = true;
            return this;
        }

        public Player Build()
        {
            Chess chess = new(CurrentPosition: BlockId,
                              Direction: Enum.Parse<Application.DataModels.Direction>(Direction.ToString()),
                              RemainSteps: 0);
            Player player = new(Id: Id,
                                    Money: Money,
                                    Chess: chess,
                                    LandContracts: LandContracts.ToArray());
            return player;
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
        public Auction? Auction { get; private set; }
        public CurrentPlayerStateBuilder(string id)
        {
            Id = id;
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

        internal CurrentPlayerStateBuilder WithAuction(string LandId, string? HighestBidderId = null, decimal? HighestPrice = null)
        {
            Auction = new Auction(LandId, HighestBidderId, HighestPrice);
            return this;
        }

        public CurrentPlayerState Build()
        {
            return new CurrentPlayerState(PlayerId: Id,
                                          IsPayToll: IsPayToll,
                                          IsBoughtLand: IsBoughtLand,
                                          IsUpgradeLand: IsUpgradeLand,
                                          Auction: Auction);
        }
    }
}