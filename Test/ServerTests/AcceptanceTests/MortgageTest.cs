using Domain;
using Server.Hubs;
using SharedLibrary;
using static Domain.Map;
using static ServerTests.Utils;

namespace ServerTests.AcceptanceTests;

[TestClass]
public class MortgageTest
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
        Given:  A 持有 A1，價值 1000元
                A 持有 5000元
        When:   A 抵押 A1
        Then:   A 持有 5000+1000*70% = 5700元
                A 持有 A1
                A1 在 10回合 後收回
        """)]
    public async Task 玩家抵押房地產()
    {
        Player A = new("A", 5000);

        const string gameId = "1";
        var monopolyBuilder = new MonopolyBuilder("1")
        .WithPlayer(
            new MonopolyPlayer(A.Id)
            .WithMoney(A.Money)
            .WithPosition("Start", Direction.Right.ToString())
            .WithLandContract("A1")
        )
        .WithCurrentPlayer(nameof(A));

        monopolyBuilder.Save(server);

        var hub = await server.CreateHubConnectionAsync(gameId, "A");

        // Act
        await hub.SendAsync(nameof(MonopolyHub.PlayerMortgage), gameId, "A", "A1");

        // Assert
        // A 抵押房地產
        hub.Verify<string, decimal, string, int>(
                       nameof(IMonopolyResponses.PlayerMortgageEvent),
                                (playerId, playerMoney, blockId, deadLine)
                                => playerId == "A" && playerMoney == 5700 && blockId == "A1" && deadLine == 10);
        hub.VerifyNoElseEvent();
    }

    [TestMethod]
    [Description(
        """
        Given:  A 持有 A1，價值 1000元，抵押中
                A 持有 1000元
        When:   A 抵押 A1
        Then:   A 無法再次抵押
                A 持有 1000元
        """)]
    public async Task 玩家不能抵押已抵押房地產()
    {
        Player A = new("A", 1000);

        const string gameId = "1";
        var monopolyBuilder = new MonopolyBuilder("1")
        .WithPlayer(
            new MonopolyPlayer(A.Id)
            .WithMoney(A.Money)
            .WithPosition("Start", Direction.Right.ToString())
            .WithLandContract("A1")
            .WithMortgage("A1")
        )
        .WithCurrentPlayer(nameof(A));

        monopolyBuilder.Save(server);

        var hub = await server.CreateHubConnectionAsync(gameId, "A");

        // Act
        await hub.SendAsync(nameof(MonopolyHub.PlayerMortgage), gameId, "A", "A1");

        // Assert
        // A 抵押房地產
        hub.Verify<string, decimal, string>(
                       nameof(IMonopolyResponses.PlayerCannotMortgageEvent),
                                (playerId, playerMoney, blockId)
                                => playerId == "A" && playerMoney == 1000 && blockId == "A1");
        hub.VerifyNoElseEvent();
    }

    [TestMethod]
    [Description(
        """
        Given:  A 持有 5000元
        When:   A 抵押 A1
        Then:   A 抵押 失敗
        """)]
    public async Task 玩家抵押非自有房地產()
    {
        Player A = new("A", 5000);

        const string gameId = "1";
        var monopolyBuilder = new MonopolyBuilder("1")
        .WithPlayer(
            new MonopolyPlayer(A.Id)
            .WithMoney(A.Money)
            .WithPosition("Start", Direction.Right.ToString())
        )
        .WithCurrentPlayer(nameof(A));

        monopolyBuilder.Save(server);

        var hub = await server.CreateHubConnectionAsync(gameId, "A");

        // Act
        await hub.SendAsync(nameof(MonopolyHub.PlayerMortgage), gameId, "A", "A1");

        // Assert
        // A 抵押房地產
        hub.Verify<string, decimal, string>(
                       nameof(IMonopolyResponses.PlayerCannotMortgageEvent),
                                (playerId, playerMoney, blockId)
                                => playerId == "A" && playerMoney == 5000 && blockId == "A1");
        hub.VerifyNoElseEvent();
    }
}


