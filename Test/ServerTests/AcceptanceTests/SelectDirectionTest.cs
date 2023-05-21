using Application.Common;
using Domain;
using Server.Hubs;
using static Domain.Map;
using static ServerTests.Utils;

namespace ServerTests.AcceptanceTests;
[TestClass]
public class SelectDirectionTest
{
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

    [TestMethod]
    [Description(
    """
        Given:  目前玩家在停車場
                玩家方向為Down
                玩家剩餘步數為 0
                玩家目前需要選擇方向
        When:   玩家選擇方向為Left
        Then:   玩家停在 停車場
                玩家剩餘步數為 0
        """)]
    public async Task 玩家選擇方向後在停車場停住()
    {
        // Arrange
        Player A = new("A", 1000);

        var monopolyBuilder = new MonopolyBuilder("1")
        .WithPlayer(
            new MonopolyPlayer(A.Id)
            .WithMoney(A.Money)
            .WithPosition("ParkingLot", Direction.Down.ToString())
        )
        .WithMockDice(new[] { 2 })
        .WithCurrentPlayer(nameof(A));

        monopolyBuilder.Save(server);

        var hub = server.CreateHubConnection();

        // Act
        await hub.SendAsync(nameof(MonopolyHub.PlayerChooseDirection), "1", "A", "Left");

        // Assert
        // A 選擇方向為 Left
        // A 停在 ParkingLot，方向為 Left，剩下 0 步

        hub.Verify<string, string>(nameof(IMonopolyResponses.PlayerChooseDirectionEvent),
            (playerId, direction) => playerId == "A" && direction == "Left");
        hub.Verify<string, int>(nameof(IMonopolyResponses.PlayerCannotMoveEvent),
                       (playerId, suspendRounds) => playerId == "A" && suspendRounds == 1);

        hub.VerifyNoElseEvent();

        var repo = server.GetRequiredService<IRepository>();
        var game = repo.FindGameById("1");
        var player = game.CurrentPlayer!;
        Assert.AreEqual("ParkingLot", player.Chess.CurrentBlock.Id);
    }
}
