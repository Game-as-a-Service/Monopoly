using Server.Hubs;
using SharedLibrary;
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
        var A = new { Id = "A", Money = 1000m };
        var B = new { Id = "B", Money = 1000m };
        var A1 = new { Id = "A1", Price = 1000m };

        const string gameId = "1";
        var monopolyBuilder = new MonopolyBuilder("1")
        .WithPlayer(
            new PlayerBuilder(A.Id)
            .WithMoney(A.Money)
            .WithPosition(A1.Id, Direction.Right)
            .Build()
        )
        .WithPlayer(
            new PlayerBuilder(B.Id)
            .WithMoney(B.Money)
            .WithLandContract(A1.Id)
            .Build()
        )
        .WithMockDice(new[] { 1 })
        .WithCurrentPlayer(new CurrentPlayerStateBuilder(A.Id).Build());

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
        var A = new { Id = "A", Money = 1000m };
        var B = new { Id = "B", Money = 1000m };
        var A1 = new { Id = "A1", Price = 1000m };
        var Jail = new { Id = "Jail" };

        const string gameId = "1";
        var monopolyBuilder = new MonopolyBuilder("1")
        .WithPlayer(
            new PlayerBuilder(A.Id)
            .WithMoney(A.Money)
            .WithPosition(A1.Id, Direction.Right)
            .Build()
        )
        .WithPlayer(
            new PlayerBuilder(B.Id)
            .WithMoney(B.Money)
            .WithPosition(Jail.Id, Direction.Right)
            .WithLandContract(A1.Id)
            .Build()
        )
        .WithMockDice(new[] { 1 })
        .WithCurrentPlayer(new CurrentPlayerStateBuilder(A.Id).Build());

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
        var A = new { Id = "A", Money = 1000m };
        var B = new { Id = "B", Money = 1000m };
        var A1 = new { Id = "A1", Price = 1000m };
        var ParkingLot = new { Id = "ParkingLot" };

        const string gameId = "1";
        var monopolyBuilder = new MonopolyBuilder("1")
        .WithPlayer(
            new PlayerBuilder(A.Id)
            .WithMoney(A.Money)
            .WithPosition(A1.Id, Direction.Right)
            .Build()
        )
        .WithPlayer(
            new PlayerBuilder(B.Id)
            .WithMoney(B.Money)
            .WithPosition(ParkingLot.Id, Direction.Right)
            .WithLandContract(A1.Id)
            .Build()
        )
        .WithMockDice(new[] { 1 })
        .WithCurrentPlayer(new CurrentPlayerStateBuilder(A.Id).Build());

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
                A 在 A1 上，持有 30元，持有土地 A3
                B 持有 1000元
                A1 是 B 的土地，價值 1000元
        When:   A 付過路費
        Then:   A 付過路費失敗
        """)]
    public async Task 玩家在別人的土地上但餘額不足以付過路費()
    {
        // Arrange
        var A = new { Id = "A", Money = 30m };
        var B = new { Id = "B", Money = 1000m };
        var A1 = new { Id = "A1", Price = 1000m };
        var A3 = new { Id = "A3" };

        const string gameId = "1";
        var monopolyBuilder = new MonopolyBuilder("1")
        .WithPlayer(
            new PlayerBuilder(A.Id)
            .WithMoney(A.Money)
            .WithPosition(A1.Id, Direction.Right)
            .WithLandContract(A3.Id)
            .Build()
        )
        .WithPlayer(
            new PlayerBuilder(B.Id)
            .WithMoney(B.Money)
            .WithLandContract(A1.Id)
            .Build()
        )
        .WithMockDice(new[] { 1 })
        .WithCurrentPlayer(new CurrentPlayerStateBuilder(A.Id).Build());

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

    [TestMethod]
    [Description(
        """
        Given:  目前輪到A
                A 在 車站1 上，持有 3000元
                B 持有 1000元
                車站1 是 B 的土地，價值 1000元
        When:   A 付過路費
        Then:   A 持有 2000元
                B 持有 2000元
        """)]
    public async Task 玩家在別人的車站上付過路費()
    {
        // Arrange
        var A = new { Id = "A", Money = 3000m };
        var B = new { Id = "B", Money = 1000m };
        var Station1 = new { Id = "Station1", Price = 1000m };

        const string gameId = "1";
        var monopolyBuilder = new MonopolyBuilder("1")
        .WithPlayer(
            new PlayerBuilder(A.Id)
            .WithMoney(A.Money)
            .WithPosition(Station1.Id, Direction.Right)
            .Build()
        )
        .WithPlayer(
            new PlayerBuilder(B.Id)
            .WithMoney(B.Money)
            .WithLandContract(Station1.Id)
            .Build()
        )
        .WithMockDice(new[] { 1 })
        .WithCurrentPlayer(new CurrentPlayerStateBuilder(A.Id).Build());

        monopolyBuilder.Save(server);

        var hub = await server.CreateHubConnectionAsync(gameId, A.Id);

        // Act
        await hub.SendAsync(nameof(MonopolyHub.PlayerPayToll), gameId, "A");

        // Assert
        // A 付過路費
        hub.Verify<string, decimal, string, decimal>(
                       nameof(IMonopolyResponses.PlayerPayTollEvent),
                                  (playerId, playerMoney, ownerId, ownerMoney)
                                  => playerId == "A" && playerMoney == 2000 && ownerId == "B" && ownerMoney == 2000);
        hub.VerifyNoElseEvent();
    }

    [TestMethod]
    [Description("""
                Given:  A 在 A1，持有 200元
                        B 持有 A2，1000元
                        A2 有 2棟 房子
                        A 擲骰得到2點
                When:   A 需付過路費 1000
                Then:   宣告 A 破產
                        A 持有 0元
                        B 持有 1000 + 200 = 1200元
                """)]
    public async Task 玩家的資產為0時破產()
    {
        // Arrange
        var A = new { Id = "A", Money = 200 };
        var B = new { Id = "B", Money = 1000 };
        var A2 = new { Id = "A2", HouseCount = 2 };

        const string gameId = "1";
        var monopolyBuilder = new MonopolyBuilder("1")
        .WithPlayer(
            new PlayerBuilder(A.Id)
            .WithMoney(A.Money)
            .WithPosition("A2", Direction.Right)
            .Build()
        )
        .WithPlayer(
            new PlayerBuilder(B.Id)
            .WithMoney(B.Money)
            .WithLandContract(A2.Id)
            .Build()
        )
        .WithMockDice(new[] { 1 })
        .WithCurrentPlayer(new CurrentPlayerStateBuilder(A.Id).Build())
        .WithLandHouse(A2.Id, A2.HouseCount);

        monopolyBuilder.Save(server);

        var hub = await server.CreateHubConnectionAsync(gameId, "A");
        // Act
        await hub.SendAsync(nameof(MonopolyHub.PlayerPayToll), "1", "A");
        // Assert
        // A 須支付過路費1000元
        // A 破產
        hub.Verify<string, decimal, string, decimal>(
                       nameof(IMonopolyResponses.PlayerPayTollEvent),
                                  (playerId, playerMoney, ownerId, ownerMoney)
                                  => playerId == "A" && playerMoney == 0 && ownerId == "B" && ownerMoney == 1200);
        hub.Verify<string>(
                       nameof(IMonopolyResponses.BankruptEvent),
                                  (playerId)
                                  => playerId == "A");
        hub.VerifyNoElseEvent();
    }
}