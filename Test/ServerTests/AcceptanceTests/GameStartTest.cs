using Application.DataModels;
using Server.Hubs;
using SharedLibrary;
using static ServerTests.Utils;

namespace ServerTests.AcceptanceTests;

[TestClass]
public class GameStartTest
{
    private MonopolyTestServer server = default!;
    private const string gameId = "1";

    [TestInitialize]
    public void Setup()
    {
        server = new MonopolyTestServer();
    }

    [TestMethod]
    [Description("""
        Given:  房主為 A, 已準備
                玩家 B, 已準備
        When:   A 開始遊戲
        Then:   遊戲開始
        """)]
    public async Task 房主成功開始遊戲()
    {
        // Arrange
        var monopolyBuilder = new MonopolyBuilder("1")
            .WithGameStage(GameStage.Preparing)
            .WithPlayer(
                new PlayerBuilder("A")
                .WithLocation(1)
                .WithRole("1")
                .WithState(Domain.PlayerState.Normal)
                .Build()
            )
            .WithPlayer(
                new PlayerBuilder("B")
                .WithLocation(2)
                .WithRole("2")
                .WithState(Domain.PlayerState.Normal)
                .Build()
            )
            .WithHost("A");

        monopolyBuilder.Save(server);

        var hub = await server.CreateHubConnectionAsync(gameId, "A");

        // Act
        await hub.SendAsync(nameof(MonopolyHub.GameStart), gameId, "A");

        // Assert
        hub.Verify<string, string>(
            nameof(IMonopolyResponses.GameStartEvent),
                (gameState, currentPlayer) => (gameState, currentPlayer) == ("Gaming", "A")
            );
    }

    [TestMethod]
    [Description("""
        Given:  房主為 A, 已準備
                沒有其他玩家
        When:   A 開始遊戲
        Then:   開始遊戲失敗
        """)]
    public async Task 人數只有1人_房主開始遊戲失敗()
    {
        // Arrange
        var monopolyBuilder = new MonopolyBuilder("1")
            .WithGameStage(GameStage.Preparing)
            .WithPlayer(
                new PlayerBuilder("A")
                .WithLocation(1)
                .WithRole("1")
                .WithState(Domain.PlayerState.Normal)
                .Build()
            )
            .WithHost("A");

        monopolyBuilder.Save(server);

        var hub = await server.CreateHubConnectionAsync(gameId, "A");

        // Act
        await hub.SendAsync(nameof(MonopolyHub.GameStart), gameId, "A");

        // Assert
        hub.Verify<string>(
            nameof(IMonopolyResponses.OnlyOnePersonEvent),
                (gameState) => gameState == "Preparing"
            );
    }

    [TestMethod]
    [Description("""
        Given:  房主為 A
                A: 位置1, 角色A, 已準備
                B: 未準備
        When:   A 開始遊戲
        Then:   開始遊戲失敗
        """)]
    public async Task 有人沒有準備_房主開始遊戲失敗()
    {
        // Arrange
        var monopolyBuilder = new MonopolyBuilder("1")
            .WithGameStage(GameStage.Preparing)
            .WithPlayer(
                new PlayerBuilder("A")
                .WithLocation(1)
                .WithRole("1")
                .WithState(Domain.PlayerState.Normal)
                .Build()
            )
            .WithPlayer(
                new PlayerBuilder("B")
                .WithRole("2")
                .WithState(Domain.PlayerState.Preparing)
                .Build()
            )
            .WithHost("A");

        monopolyBuilder.Save(server);

        var hub = await server.CreateHubConnectionAsync(gameId, "A");

        // Act
        await hub.SendAsync(nameof(MonopolyHub.GameStart), gameId, "A");

        // Assert
        hub.Verify<string, string[]>(
            nameof(IMonopolyResponses.SomePlayersPreparingEvent),
                (gameState, players) => gameState == "Preparing" && players.OrderBy(x => x).SequenceEqual(new[] { "B" }.OrderBy(x => x))
            );
    }
}
