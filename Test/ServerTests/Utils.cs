using Application.Common;
using Domain;
using Domain.Interfaces;
using Domain.Maps;
using Moq;
using Server.Hubs;
using static Domain.Map;

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

        public List<MonopolyPlayer> Players { get; private set; } = new();

        public Monopoly Game { get; private set; }

        public int[] Dices { get; private set; }

        public string CurrentPlayer { get; private set; }

        public MonopolyBuilder(string id)
        {
            GameId = id;
        }

        public MonopolyBuilder WithPlayer(MonopolyPlayer player)
        {
            Players.Add(player);
            return this;
        }

        public MonopolyBuilder WithMockDice(int[] dices)
        {
            Dices = dices;
            return this;
        }

        public MonopolyBuilder WithCurrentPlayer(string playerId)
        {
            CurrentPlayer = playerId;
            return this;
        }

        public Monopoly Build()
        {
            var map = new SevenXSevenMap();
            var monopoly = new Monopoly(GameId, map, (Dices is null ? null : MockDice(Dices)));
            Players.ForEach(p =>
            {
                var player = new Player(p.Id, p.Money);
                var block = map.FindBlockById(p.BlockId);
                var direction = (Direction)Enum.Parse(typeof(Direction), p.Direction);
                player.Chess = new Chess(player, map, block, direction);
                p.LandContracts.ForEach(l =>
                {
                    player.AddLandContract(new LandContract(player, (Land)map.FindBlockById(l)));

                    Land land = (Land)map.FindBlockById(l);
                    for (int i = 0; i < p.House[l]; i++) land.Upgrade();
                });
                monopoly.AddPlayer(player, p.BlockId, direction);
                if (CurrentPlayer == player.Id)
                {
                    monopoly.CurrentPlayer = player;
                }
            });
            //monopoly.Initial();
            return monopoly;
        }

        internal void Save(MonopolyTestServer server)
        {
            var monopoly = this.Build();
            server.GetRequiredService<IRepository>().Save(monopoly);
        }
    }

    public class MonopolyPlayer
    {
        public string Id { get; set; }
        public decimal Money { get; set; }
        public string BlockId { get; set; }
        public string Direction { get; set; }
        public List<string> LandContracts { get; set; }
        public IDictionary<string, int> House = new Dictionary<string, int>();

        public MonopolyPlayer(string id)
        {
            Id = id;
            Money = 15000;
            BlockId = "StartPoint";
            Direction = "Right";
            LandContracts = new();
        }

        public MonopolyPlayer WithMoney(decimal money)
        {
            Money = money;
            return this;
        }

        public MonopolyPlayer WithPosition(string blockId, string direction)
        {
            BlockId = blockId;
            Direction = direction;
            return this;
        }

        public MonopolyPlayer WithLandContract(string landId, int house = 0)
        {
            LandContracts.Add(landId);
            House.Add(landId, house);
            return this;
        }
    }
}