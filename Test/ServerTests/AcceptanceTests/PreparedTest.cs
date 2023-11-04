using Application.DataModels;
using Server.Hubs;
using SharedLibrary;
using static ServerTests.Utils;

namespace ServerTests.AcceptanceTests;

[TestClass]
public class PreparedTest
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
        Given:  房主為 A
                A: 位置1, 角色A
                B: 位置2, 角色B
        When:   B 按下準備
        Then:   B 的準備狀態:已準備
        """)]
    public async Task 玩家成功準備()
    {
        // Arrange
        var monopolyBuilder = new MonopolyBuilder("1")
            .WithGameStage(GameStage.Preparing)
            .WithPlayer(
                new PlayerBuilder("A")
                .WithLocation(1)
                .WithRole("1")
                .WithState(Domain.PlayerState.Preparing)
                .Build()
            )
            .WithPlayer(
                new PlayerBuilder("B")
                .WithLocation(2)
                .WithRole("2")
                .WithState(Domain.PlayerState.Preparing)
                .Build()
            );

        monopolyBuilder.Save(server);

        var hub = await server.CreateHubConnectionAsync(gameId, "A");

        // Act
        await hub.SendAsync(nameof(MonopolyHub.PlayerPrepare), gameId, "B");

        // Assert
        hub.Verify<string, string>(
            nameof(IMonopolyResponses.PlayerPrepareEvent),
                (playerId, playerState) => (playerId, playerState) == ("B", "Normal")
            );
    }

    [TestMethod]
    [Description("""
        Given:  房主為 A
                A: 位置1, 角色A, 已準備
        When:   A 取消準備
        Then:   A 的準備狀態:準備中
        """)]
    public async Task 玩家取消準備()
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
            );

        monopolyBuilder.Save(server);

        var hub = await server.CreateHubConnectionAsync(gameId, "A");

        // Act
        await hub.SendAsync(nameof(MonopolyHub.PlayerPrepare), gameId, "A");

        // Assert
        hub.Verify<string, string>(
            nameof(IMonopolyResponses.PlayerPrepareEvent),
                (playerId, playerState) => (playerId, playerState) == ("A", "Preparing")
            );
    }

    [TestMethod]
    [Description("""
        Given:  房主為 A
                A: 位置1, 角色A, 已準備
                B: 位置未選擇
        When:   B 按下準備
        Then:   提醒無法準備, B 的準備狀態:準備中
        """)]
    public async Task 玩家未選擇位置按下準備()
    {
        // Arrange
        var monopolyBuilder = new MonopolyBuilder("1")
            .WithGameStage(GameStage.Preparing)
            .WithPlayer(
                new PlayerBuilder("A")
                .WithLocation(1)
                .WithRole("1")
                .WithState(Domain.PlayerState.Preparing)
                .Build()
            )
            .WithPlayer(
                new PlayerBuilder("B")
                .WithRole("2")
                .WithState(Domain.PlayerState.Preparing)
                .Build()
            );

        monopolyBuilder.Save(server);

        var hub = await server.CreateHubConnectionAsync(gameId, "A");

        // Act
        await hub.SendAsync(nameof(MonopolyHub.PlayerPrepare), gameId, "B");

        // Assert
        hub.Verify<string, string, string, int>(
            nameof(IMonopolyResponses.PlayerCannotPrepareEvent),
                (playerId, playerState, roleId, locationId) => (playerId, playerState, roleId, locationId) == ("B", "Preparing", "2", 0)
            );
    }

    [TestMethod]
    [Description("""
        Given:  房主為 A
                A: 位置1, 角色A, 已準備
                B: 位置2, 角色未選擇
        When:   B 按下準備
        Then:   提醒無法準備, B 的準備狀態:準備中
        """)]
    public async Task 玩家未選擇角色按下準備()
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
                .WithState(Domain.PlayerState.Preparing)
                .Build()
            );

        monopolyBuilder.Save(server);

        var hub = await server.CreateHubConnectionAsync(gameId, "A");

        // Act
        await hub.SendAsync(nameof(MonopolyHub.PlayerPrepare), gameId, "B");

        // Assert
        hub.Verify<string, string, string, int>(
            nameof(IMonopolyResponses.PlayerCannotPrepareEvent),
                (playerId, playerState, roleId, locationId) => (playerId, playerState, roleId, locationId) == ("B", "Preparing", null, 2)
            );
    }
}
