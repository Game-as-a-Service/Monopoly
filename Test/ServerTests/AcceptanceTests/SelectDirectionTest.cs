using Application.Common;
using Domain;
using Domain.Maps;
using Server.Hubs;
using static Domain.Map;

namespace ServerTests.AcceptanceTests;
[TestClass]
public class SelectDirectionTest {
    private MonopolyTestServer server = default!;

    [TestInitialize]
    public void Setup()
    {
        server = new MonopolyTestServer();
    }

    [TestMethod]
    [Description(
    """
        Given:  目前玩家在監獄
                玩家方向為Down
                玩家剩餘步數為 0
                玩家目前需要選擇方向
        When:   玩家選擇方向為Left
        Then:   玩家停在 監獄
                玩家剩餘步數為 0
                玩家下一回合無法行動
        """)]
    public async Task 玩家選擇方向後在監獄停住()
    {
        // Arrange
        Player A = new("A", 1000);
        SetupMonopoly("1", A, "Jail", Direction.Down, new[] { 2 }, remainingSteps: 0);

        var hub = server.CreateHubConnection();

        // Act
        await hub.SendAsync(nameof(MonopolyHub.PlayerChooseDirection), "1", "A", "Left");

        // Assert
        // A 選擇方向為 Left
        // A 停在 Jail，方向為 Left，剩下 0 步
        // A 下一回合無法行動，暫停2回合
        hub.Verify<string, string>(nameof(IMonopolyResponses.PlayerChooseDirectionEvent),
            (playerId, direction) => playerId == "A" && direction == "Left");
        hub.Verify<string, int>(nameof(IMonopolyResponses.PlayerCannotMoveEvent),
                       (playerId, suspendRounds) => playerId == "A" && suspendRounds == 2);
        hub.VerifyNoElseEvent();

        var repo = server.GetRequiredService<IRepository>();
        var game = repo.FindGameById("1");
        var player = game.CurrentPlayer!;
        Assert.AreEqual("Jail", player.Chess.CurrentBlock.Id);
    }

    private void SetupMonopoly(string gameId, Player player, string initialBlockId, Direction initialDirection, int[] dices, string[] landContracts = default!, int remainingSteps = 0)
    {
        var repo = server.GetRequiredService<IRepository>();
        var map = new SevenXSevenMap();
        var game = new Monopoly(gameId, map, Utils.MockDice(dices));

        game.AddPlayer(player, initialBlockId, initialDirection);
        if (landContracts != null)
        {
            foreach (var landContract in landContracts)
            {
                Land? land = map.FindBlockById(landContract) as Land;
                player.AddLandContract(new(player, land));
            }
        }

        game.Initial();
        repo.Save(game);
    }
}
