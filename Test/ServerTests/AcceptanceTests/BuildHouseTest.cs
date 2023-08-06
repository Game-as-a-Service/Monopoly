using Domain;
using Server.Hubs;
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
                                  (playId, blockId, playerMoney, house)
                                  => playId == "A" && blockId == "A1" && playerMoney == 1000 && house == 2);
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
                                  (playId, blockId, house)
                                  => playId == "A" && blockId == "A1" && house == 5);
        hub.VerifyNoElseEvent();
    }
}
