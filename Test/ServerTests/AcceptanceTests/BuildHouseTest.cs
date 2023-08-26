using Domain;
using Server.Hubs;
using SharedLibrary;
using static Domain.Map;
using static ServerTests.Utils;

namespace ServerTests.AcceptanceTests;

[TestClass]
public class BuildHouseTest
{
    private MonopolyTestServer server = default!;

    [TestInitialize]

    public void SetUp()
    {
        server = new MonopolyTestServer();
    }

    [TestMethod]
    [Description(
        """
        Given:  A 持有 A1
                A1 有 1間房子，購買價 1000元
                A 持有 2000元，在 A1 上
        When:   A 蓋房子
        Then:   A 持有 1000元
                A1 有 2間房子
        """)]
    public async Task 玩家在自己的土地蓋房子()
    {
        Player A = new("A", 2000);

        const string gameId = "1";
        var monopolyBuilder = new MonopolyBuilder("1")
        .WithPlayer(
            new MonopolyPlayer(A.Id)
            .WithMoney(A.Money)
            .WithPosition("A1", Direction.Right.ToString())
            .WithLandContract("A1", 1)
        )
        .WithCurrentPlayer(nameof(A));

        monopolyBuilder.Save(server);

        var hub = await server.CreateHubConnectionAsync(gameId, "A");

        // Act
        await hub.SendAsync(nameof(MonopolyHub.PlayerBuildHouse), gameId, "A");

        // Assert
        // A 蓋房子
        hub.Verify<string, string, decimal, int>(
                       nameof(IMonopolyResponses.PlayerBuildHouseEvent),
                                  (playerId, blockId, playerMoney, house)
                                  => playerId == "A" && blockId == "A1" && playerMoney == 1000 && house == 2);
        hub.VerifyNoElseEvent();
    }

    [TestMethod]
    [Description(
        """
        Given:  A 持有 A1
                A1 有 5間房子，購買價 1000元
                A 持有 2000元，在 A1 上
        When:   A 蓋房子
        Then:   A 不能蓋房子
        """)]
    public async Task 土地最多蓋5間房子()
    {
        Player A = new("A", 2000);

        const string gameId = "1";
        var monopolyBuilder = new MonopolyBuilder("1")
        .WithPlayer(
            new MonopolyPlayer(A.Id)
            .WithMoney(A.Money)
            .WithPosition("A1", Direction.Right.ToString())
            .WithLandContract("A1", 5)
        )
        .WithCurrentPlayer(nameof(A));

        monopolyBuilder.Save(server);

        var hub = await server.CreateHubConnectionAsync(gameId, "A");

        // Act
        await hub.SendAsync(nameof(MonopolyHub.PlayerBuildHouse), gameId, "A");

        // Assert
        // A 蓋房子
        hub.Verify<string, string, int>(
                       nameof(IMonopolyResponses.HouseMaxEvent),
                                  (playerId, blockId, house)
                                  => playerId == "A" && blockId == "A1" && house == 5);
        hub.VerifyNoElseEvent();
    }

    [TestMethod]
    [Description(
        """
        Given:  A 持有 車站1，購買價 1000元
                A 持有 1000元，在 車站1 上
        When:   A 蓋房子
        Then:   A 不能蓋房子
        """)]
    public async Task 車站不能蓋房子()
    {
        Player A = new("A", 1000);

        const string gameId = "1";
        var monopolyBuilder = new MonopolyBuilder("1")
        .WithPlayer(
            new MonopolyPlayer(A.Id)
            .WithMoney(A.Money)
            .WithPosition("Station1", Direction.Right.ToString())
            .WithLandContract("Station1")
        )
        .WithCurrentPlayer(nameof(A));

        monopolyBuilder.Save(server);

        var hub = await server.CreateHubConnectionAsync(gameId, "A");

        // Act
        await hub.SendAsync(nameof(MonopolyHub.PlayerBuildHouse), gameId, "A");

        // Assert
        // A 蓋房子
        hub.Verify<string, string>(
                       nameof(IMonopolyResponses.PlayerCannotBuildHouseEvent),
                                  (playerId, blockId)
                                  => playerId == "A" && blockId == "Station1");
        hub.VerifyNoElseEvent();
    }

    [TestMethod]
    [Description(
        """
        Given:  A 持有 A1，購買價 1000元
                A 持有 1000元，在 A1 上
                本回合已經蓋過房子了
        When:   A 蓋房子
        Then:   A 不能蓋房子
        """)]
    public async Task 一回合內玩家不能重複蓋房子()
    {
        Player A = new("A", 1000);

        const string gameId = "1";
        var monopolyBuilder = new MonopolyBuilder("1")
        .WithPlayer(
            new MonopolyPlayer(A.Id)
            .WithMoney(A.Money)
            .WithPosition("A1", Direction.Right.ToString())
            .WithLandContract("A1", 1)
        )
        .WithCurrentPlayer(nameof(A), upgrade : true);

        monopolyBuilder.Save(server);

        var hub = await server.CreateHubConnectionAsync(gameId, "A");

        // Act
        await hub.SendAsync(nameof(MonopolyHub.PlayerBuildHouse), gameId, "A");

        // Assert
        // A 蓋房子
        hub.Verify<string, string>(
                       nameof(IMonopolyResponses.PlayerCannotBuildHouseEvent),
                                  (playerId, blockId)
                                  => playerId == "A" && blockId == "A1");
        hub.VerifyNoElseEvent();
    }

    [TestMethod]
    [Description(
        """
        Given:  A 持有 A1，購買價 1000元
                A 持有 1000元，在 A1 上
                A1 於本回合購買 
        When:   A 蓋房子
        Then:   A 不能蓋房子
        """)]
    public async Task 一回合內玩家買土地後不能馬上蓋房子()
    {
        Player A = new("A", 1000);

        const string gameId = "1";
        var monopolyBuilder = new MonopolyBuilder("1")
        .WithPlayer(
            new MonopolyPlayer(A.Id)
            .WithMoney(A.Money)
            .WithPosition("A1", Direction.Right.ToString())
        )
        .WithCurrentPlayer(nameof(A), buyLand : "A1");

        monopolyBuilder.Save(server);

        var hub = await server.CreateHubConnectionAsync(gameId, "A");

        // Act
        await hub.SendAsync(nameof(MonopolyHub.PlayerBuildHouse), gameId, "A");

        // Assert
        // A 蓋房子
        hub.Verify<string, string>(
                       nameof(IMonopolyResponses.PlayerBuyBlockEvent),
                                  (playerId, blockId)
                                  => playerId == "A" && blockId == "A1");
        hub.Verify<string, string>(
                       nameof(IMonopolyResponses.PlayerCannotBuildHouseEvent),
                                  (playerId, blockId)
                                  => playerId == "A" && blockId == "A1");
        hub.VerifyNoElseEvent();
    }

    [TestMethod]
    [Description("""
                Given:  目前玩家在A1
                        玩家持有A2
                        A2房子不足5間
                        玩家擲骰得到2點
                        玩家移動到 A2
                When:   A 蓋房子
                Then:   A 不能蓋房子
                """)]
    public async Task 玩家擲骰後移動棋子到自己已抵押土地()
    {
        // Arrange
        Player A = new("A");

        const string gameId = "1";
        var monopolyBuilder = new MonopolyBuilder("1")
        .WithPlayer(
            new MonopolyPlayer(A.Id)
            .WithMoney(A.Money)
            .WithPosition("A1", Direction.Right.ToString())
            .WithLandContract("A2")
            .WithMortgage("A2")
        )
        .WithMockDice(new[] { 2 })
        .WithCurrentPlayer(nameof(A), rollDice : true);

        monopolyBuilder.Save(server);

        var hub = await server.CreateHubConnectionAsync(gameId, "A");
        // Act
        await hub.SendAsync(nameof(MonopolyHub.PlayerBuildHouse), "1", "A");
        // Assert
        // A 蓋房子失敗
        hub.Verify<string, int>(
                       nameof(IMonopolyResponses.PlayerRolledDiceEvent),
                                  (playerId, diceCount) => playerId == "A" && diceCount == 2);
        VerifyChessMovedEvent(hub, "A", "Station1", "Right", 1);
        VerifyChessMovedEvent(hub, "A", "A2", "Right", 0);
        hub.Verify<string, string>(
                       nameof(IMonopolyResponses.PlayerCannotBuildHouseEvent),
                                  (playerId, blockId)
                                  => playerId == "A" && blockId == "A2");
        hub.VerifyNoElseEvent();
    }
}
