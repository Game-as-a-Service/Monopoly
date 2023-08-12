using Domain;
using Server.Hubs;
using static Domain.Map;
using static ServerTests.Utils;

namespace ServerTests.AcceptanceTests;

[TestClass]
public class RedeemTest
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
        Given:  A 持有 A1，價值 1000元，有 2棟房
                A 持有 5000元
                A1 抵押中
        When:   A 贖回 A1
        Then:   A 持有 5000 - 3000 = 2000元
                A 持有 A1
                A1 不在抵押狀態
        """)]
    public async Task 玩家贖回房地產()
    {
        Player A = new("A", 5000);

        const string gameId = "1";
        var monopolyBuilder = new MonopolyBuilder("1")
        .WithPlayer(
            new MonopolyPlayer(A.Id)
            .WithMoney(A.Money)
            .WithPosition("Start", Direction.Right.ToString())
            .WithLandContract("A1", 2)
            .WithMortgage("A1")
        )
        .WithCurrentPlayer(nameof(A));

        monopolyBuilder.Save(server);

        var hub = await server.CreateHubConnectionAsync(gameId, "A");

        // Act
        await hub.SendAsync(nameof(MonopolyHub.PlayerRedeem), gameId, "A", "A1");

        // Assert
        // A 贖回房地產
        hub.Verify<string, decimal, string>(
                       nameof(IMonopolyResponses.PlayerRedeemEvent),
                                (playerId, playerMoney, blockId)
                                => playerId == "A" && playerMoney == 2000 && blockId == "A1");
        hub.VerifyNoElseEvent();
    }

    [TestMethod]
    [Description(
        """
        Given:  A 持有 A1，價值 1000元，有 2棟房
                A 持有 2000元
                A1 抵押中
        When:   A 贖回 A1
        Then:   A 持有 2000元
                A 持有 A1
                A1 抵押中
        """)]
    public async Task 玩家餘額不足以贖回房地產()
    {
        Player A = new("A", 2000);

        const string gameId = "1";
        var monopolyBuilder = new MonopolyBuilder("1")
        .WithPlayer(
            new MonopolyPlayer(A.Id)
            .WithMoney(A.Money)
            .WithPosition("Start", Direction.Right.ToString())
            .WithLandContract("A1", 2)
            .WithMortgage("A1")
        )
        .WithCurrentPlayer(nameof(A));

        monopolyBuilder.Save(server);

        var hub = await server.CreateHubConnectionAsync(gameId, "A");

        // Act
        await hub.SendAsync(nameof(MonopolyHub.PlayerRedeem), gameId, "A", "A1");

        // Assert
        // A 贖回房地產
        hub.Verify<string, decimal, string, decimal>(
                       nameof(IMonopolyResponses.PlayerTooPoorToRedeemEvent),
                                (playerId, playerMoney, blockId, redeemPrice)
                                => playerId == "A" && playerMoney == 2000 && blockId == "A1" && redeemPrice == 3000);
        hub.VerifyNoElseEvent();
    }
}