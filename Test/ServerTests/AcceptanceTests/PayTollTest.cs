using Domain;
using Server.Hubs;
using static Domain.Map;
using static ServerTests.Utils;

namespace ServerTests.AcceptanceTests;

[TestClass]
public class PayTollTest
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
        Given:  目前輪到A
                A 在 A1 上，持有 1000元
                B 持有 1000元
                A1 是 B 的土地，價值 1000元
        When:   A 付過路費
        Then:   A 持有 950元
                B 持有 1050元
        """)]
    public async Task 玩家在別人的土地上付過路費()
    {
        // Arrange
        Player A = new("A", 1000);
        Player B = new("B", 1000);

        const string gameId = "1";
        var monopolyBuilder = new MonopolyBuilder("1")
        .WithPlayer(
            new MonopolyPlayer(A.Id)
            .WithMoney(A.Money)
            .WithPosition("A1", Direction.Right.ToString())
        )
        .WithPlayer(
            new MonopolyPlayer(B.Id)
            .WithMoney(B.Money)
            .WithPosition("A1", Direction.Right.ToString())
            .WithLandContract("A1")
        )
        .WithCurrentPlayer(nameof(A));

        monopolyBuilder.Save(server);

        var hub = await server.CreateHubConnectionAsync(gameId, A.Id);

        // Act
        await hub.SendAsync(nameof(MonopolyHub.PlayerPayToll), gameId, "A");

        // Assert
        // A 付過路費
        hub.Verify<string, decimal, string, decimal>(
                       nameof(IMonopolyResponses.PlayerPayTollEvent),
                                  (playerId, playerMoney, ownerId, ownerMoney) 
                                  => playerId == "A" && playerMoney == 950 && ownerId == "B" && ownerMoney == 1050);
        hub.VerifyNoElseEvent();
    }

    [TestMethod]
    [Description(
        """
        Given:  目前輪到A
                A 在 A1 上，持有 1000元
                B 持有 1000元
                B 在 監獄
                A1 是 B 的土地，價值 1000元
        When:   A 付過路費
        Then:   A 無須付過路費
        """)]
    public async Task 地主在監獄中玩家無須付過路費()
    {
        // Arrange
        Player A = new("A", 1000);
        Player B = new("B", 1000);

        const string gameId = "1";
        var monopolyBuilder = new MonopolyBuilder("1")
        .WithPlayer(
            new MonopolyPlayer(A.Id)
            .WithMoney(A.Money)
            .WithPosition("A1", Direction.Right.ToString())
        )
        .WithPlayer(
            new MonopolyPlayer(B.Id)
            .WithMoney(B.Money)
            .WithPosition("Jail", Direction.Right.ToString())
            .WithLandContract("A1")
        )
        .WithCurrentPlayer(nameof(A));

        monopolyBuilder.Save(server);

        var hub = await server.CreateHubConnectionAsync(gameId, A.Id);

        // Act
        await hub.SendAsync(nameof(MonopolyHub.PlayerPayToll), gameId, "A");

        // Assert
        // A 付過路費
        hub.Verify<string, decimal>(
                       nameof(IMonopolyResponses.PlayerDoesntNeedToPayTollEvent),
                                  (playerId, playerMoney) 
                                  => playerId == "A" && playerMoney == 1000);
        hub.VerifyNoElseEvent();
    }

    [TestMethod]
    [Description(
        """
        Given:  目前輪到A
                A 在 A1 上，持有 1000元
                B 持有 1000元
                B 在 停車場
                A1 是 B 的土地，價值 1000元
        When:   A 付過路費
        Then:   A 無須付過路費
        """)]
    public async Task 地主在停車場玩家無須付過路費()
    {
        // Arrange
        Player A = new("A", 1000);
        Player B = new("B", 1000);

        const string gameId = "1";
        var monopolyBuilder = new MonopolyBuilder("1")
        .WithPlayer(
            new MonopolyPlayer(A.Id)
            .WithMoney(A.Money)
            .WithPosition("A1", Direction.Right.ToString())
        )
        .WithPlayer(
            new MonopolyPlayer(B.Id)
            .WithMoney(B.Money)
            .WithPosition("ParkingLot", Direction.Right.ToString())
            .WithLandContract("A1")
        )
        .WithCurrentPlayer(nameof(A));

        monopolyBuilder.Save(server);

        var hub = await server.CreateHubConnectionAsync(gameId, A.Id);

        // Act
        await hub.SendAsync(nameof(MonopolyHub.PlayerPayToll), gameId, "A");

        // Assert
        // A 付過路費
        hub.Verify<string, decimal>(
                       nameof(IMonopolyResponses.PlayerDoesntNeedToPayTollEvent),
                                  (playerId, playerMoney) 
                                  => playerId == "A" && playerMoney == 1000);
        hub.VerifyNoElseEvent();
    }

    [TestMethod]
    [Description(
        """
        Given:  目前輪到A
                A 在 A1 上，持有 30元
                B 持有 1000元
                A1 是 B 的土地，價值 1000元
        When:   A 付過路費
        Then:   A 付過路費失敗
        """)]
    public async Task 玩家在別人的土地上但餘額不足以付過路費()
    {
        // Arrange
        Player A = new("A", 30);
        Player B = new("B", 1000);

        const string gameId = "1";
        var monopolyBuilder = new MonopolyBuilder("1")
        .WithPlayer(
            new MonopolyPlayer(A.Id)
            .WithMoney(A.Money)
            .WithPosition("A1", Direction.Right.ToString())
        )
        .WithPlayer(
            new MonopolyPlayer(B.Id)
            .WithMoney(B.Money)
            .WithPosition("A1", Direction.Right.ToString())
            .WithLandContract("A1")
        )
        .WithCurrentPlayer(nameof(A));

        monopolyBuilder.Save(server);

        var hub = await server.CreateHubConnectionAsync(gameId, A.Id);

        // Act
        await hub.SendAsync(nameof(MonopolyHub.PlayerPayToll), gameId, "A");

        // Assert
        // A 付過路費
        hub.Verify<string, decimal, decimal>(
                       nameof(IMonopolyResponses.PlayerTooPoorToPayTollEvent),
                                  (playerId, playerMoney, toll) 
                                  => playerId == "A" && playerMoney == 30 && toll == 50);
        hub.VerifyNoElseEvent();
    }
}