using Domain;
using Server.Hubs;
using SharedLibrary;
using static Domain.Map;
using static ServerTests.Utils;

namespace ServerTests.AcceptanceTests;

[TestClass]
public class AuctionTest
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
                A 持有 2000元
                B 持有 2000元
                A 拍賣 A1，初始金額為 (1000 + 1000 * 2) * 50% = 1500
        When:   B 喊價 1500
        Then:   B 喊價 成功
        """)]
    public async Task 玩家喊價成功()
    {
        Player A = new("A", 2000);
        Player B = new("B", 2000);

        const string gameId = "1";
        var monopolyBuilder = new MonopolyBuilder("1")
        .WithPlayer(
            new MonopolyPlayer(A.Id)
            .WithMoney(A.Money)
            .WithPosition("Start", Direction.Right.ToString())
            .WithLandContract("A1", 2)
        )
        .WithPlayer(
            new MonopolyPlayer(B.Id)
            .WithMoney(B.Money)
            .WithPosition("Start", Direction.Right.ToString())
        )
        .WithCurrentPlayer(nameof(A), auction : "A1");

        monopolyBuilder.Save(server);

        var hub = await server.CreateHubConnectionAsync(gameId, "A");

        // Act
        await hub.SendAsync(nameof(MonopolyHub.PlayerBid), gameId, "B", 1500);

        // Assert
        // B 喊價
        hub.Verify<string, string, decimal>(
                       nameof(IMonopolyResponses.PlayerBidEvent),
                                (playerId, blockId, highestPrice)
                                => playerId == "B" && blockId == "A1" && highestPrice == 1500);
        hub.VerifyNoElseEvent();
    }

    [TestMethod]
    [Description(
        """
        Given:  A 持有 A1，價值 1000元，有 1棟房
                A 持有 1000元
                B 持有 2000元
                A 拍賣 A1，初始金額為 (1000 + 1000 * 1) * 50% = 1000
        When:   B 喊價 3000
        Then:   B 喊價 失敗
        """)]
    public async Task 玩家不能喊出比自己現金還高的金額()
    {
        Player A = new("A", 1000);
        Player B = new("B", 2000);

        const string gameId = "1";
        var monopolyBuilder = new MonopolyBuilder("1")
        .WithPlayer(
            new MonopolyPlayer(A.Id)
            .WithMoney(A.Money)
            .WithPosition("Start", Direction.Right.ToString())
            .WithLandContract("A1", 1)
        )
        .WithPlayer(
            new MonopolyPlayer(B.Id)
            .WithMoney(B.Money)
            .WithPosition("Start", Direction.Right.ToString())
        )
        .WithCurrentPlayer(nameof(A), auction : "A1");

        monopolyBuilder.Save(server);

        var hub = await server.CreateHubConnectionAsync(gameId, "A");

        // Act
        await hub.SendAsync(nameof(MonopolyHub.PlayerBid), gameId, "B", 3000);

        // Assert
        // B 喊價
        hub.Verify<string, decimal, decimal, decimal>(
                       nameof(IMonopolyResponses.PlayerTooPoorToBidEvent),
                                (playerId, playerMoney, bidPrice, highestPrice)
                                => playerId == "B" && playerMoney == 2000 && bidPrice == 3000 && highestPrice == 1000);
        hub.VerifyNoElseEvent();
    }

    [TestMethod]
    [Description(
        """
        Given:  A 持有 A1，價值 1000元，有 1棟房
                A 持有 1000元
                B 持有 2000元
                A 拍賣 A1，初始金額為 (1000 + 1000 * 1) * 50% = 1000
        When:   B 喊價 800
        Then:   B 喊價 失敗
        """)]
    public async Task 玩家不能喊出比當前拍賣金額更低的金額()
    {
        Player A = new("A", 1000);
        Player B = new("B", 2000);

        const string gameId = "1";
        var monopolyBuilder = new MonopolyBuilder("1")
        .WithPlayer(
            new MonopolyPlayer(A.Id)
            .WithMoney(A.Money)
            .WithPosition("Start", Direction.Right.ToString())
            .WithLandContract("A1", 1)
        )
        .WithPlayer(
            new MonopolyPlayer(B.Id)
            .WithMoney(B.Money)
            .WithPosition("Start", Direction.Right.ToString())
        )
        .WithCurrentPlayer(nameof(A), auction : "A1");

        monopolyBuilder.Save(server);

        var hub = await server.CreateHubConnectionAsync(gameId, "A");

        // Act
        await hub.SendAsync(nameof(MonopolyHub.PlayerBid), gameId, "B", 800);

        // Assert
        // B 喊價
        hub.Verify<string, string, decimal, decimal>(
                       nameof(IMonopolyResponses.PlayerBidFailEvent),
                                (playerId, blockId, bidPrice, highestPrice)
                                => playerId == "B" && blockId == "A1" && bidPrice == 800 && highestPrice == 1000);
        hub.VerifyNoElseEvent();
    }
}