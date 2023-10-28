using Server.Hubs;
using SharedLibrary;
using static ServerTests.Utils;

namespace ServerTests.AcceptanceTests;

[TestClass]
[Ignore]
public class SettlementTest
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
        Given:  A 持有 1000元
                B 破產
                C 破產
        When:   遊戲結算
        Then:   結算名次
        """)]
    public async Task 玩家破產遊戲結算()
    {
        // Arrange
        var A = new { Id = "A", Money = 1000m };
        var B = new { Id = "B", Money = 0m };
        var C = new { Id = "C", Money = 0m };

        const string gameId = "1";
        var monopolyBuilder = new MonopolyBuilder("1")
        .WithPlayer(
            new PlayerBuilder(A.Id)
            .WithMoney(A.Money)
            .Build()
        )
        .WithPlayer(
            new PlayerBuilder(B.Id)
            .WithMoney(B.Money)
            .WithBankrupt(5)
            .Build()
        )
        .WithPlayer(
            new PlayerBuilder(C.Id)
            .WithMoney(C.Money)
            .WithBankrupt(6)
            .Build()
        )
        .WithMockDice(new[] { 1 })
        .WithCurrentPlayer(new CurrentPlayerStateBuilder(A.Id).Build());

        monopolyBuilder.Save(server);

        var hub = await server.CreateHubConnectionAsync(gameId, "A");

        // Act
        await hub.SendAsync(nameof(MonopolyHub.Settlement), gameId, "A");

        // Assert
        // 遊戲結算
        hub.Verify<string, int>(
                       nameof(IMonopolyResponses.SettlementEvent),
                                  (playerId, rank)
                                  => playerId == "B" && rank == 3);
        hub.Verify<string, int>(
                       nameof(IMonopolyResponses.SettlementEvent),
                                  (playerId, rank)
                                  => playerId == "C" && rank == 2);
        hub.Verify<string, int>(
                       nameof(IMonopolyResponses.SettlementEvent),
                                  (playerId, rank)
                                  => playerId == "A" && rank == 1);
        hub.VerifyNoElseEvent();
    }

    [TestMethod]
    [Description(
        """
        Given:  A 持有 800元，持有 A2
                B 持有 1000元
                C 持有 600元
        When:   遊戲結算
        Then:   結算名次
        """)]
    public async Task 遊戲結束結算()
    {
        // Arrange
        var A = new { Id = "A", Money = 800m };
        var B = new { Id = "B", Money = 1000m };
        var C = new { Id = "C", Money = 600m };
        var A2 = new { Id = "A2", Price = 1000m, IsMortgage = true };

        const string gameId = "1";
        var monopolyBuilder = new MonopolyBuilder("1")
        .WithPlayer(
            new PlayerBuilder(A.Id)
            .WithMoney(A.Money)
            .WithLandContract(A2.Id, InMortgage: A2.IsMortgage)
            .Build()
        )
        .WithPlayer(
            new PlayerBuilder(B.Id)
            .WithMoney(B.Money)
            .Build()
        )
        .WithPlayer(
            new PlayerBuilder(C.Id)
            .WithMoney(C.Money)
            .Build()
        )
        .WithMockDice(new[] { 1 })
        .WithCurrentPlayer(new CurrentPlayerStateBuilder(A.Id).Build());

        monopolyBuilder.Save(server);

        var hub = await server.CreateHubConnectionAsync(gameId, "A");

        // Act
        await hub.SendAsync(nameof(MonopolyHub.Settlement), gameId, "A");

        // Assert
        // 遊戲結算
        hub.Verify<string, int>(
                       nameof(IMonopolyResponses.SettlementEvent),
                                  (playerId, rank)
                                  => playerId == "C" && rank == 3);
        hub.Verify<string, int>(
                       nameof(IMonopolyResponses.SettlementEvent),
                                  (playerId, rank)
                                  => playerId == "B" && rank == 2);
        hub.Verify<string, int>(
                       nameof(IMonopolyResponses.SettlementEvent),
                                  (playerId, rank)
                                  => playerId == "A" && rank == 1);
        
        hub.VerifyNoElseEvent();
    }

    [TestMethod]
    [Description(
        """
        Given:  A 持有 800元，持有 A2，A2抵押中
                B 持有 1000元
                C 持有 600元
        When:   遊戲結算
        Then:   結算名次
        """)]
    public async Task 遊戲結束結算含抵押土地()
    {
        // Arrange
        var A = new { Id = "A", Money = 800m };
        var B = new { Id = "B", Money = 1000m };
        var C = new { Id = "C", Money = 600m };
        var A2 = new { Id = "A2", Price = 1000m, IsMortgage = true }; 

        const string gameId = "1";
        var monopolyBuilder = new MonopolyBuilder("1")
        .WithPlayer(
            new PlayerBuilder(A.Id)
            .WithMoney(A.Money)
            .WithLandContract(A2.Id, InMortgage: A2.IsMortgage)
            .Build()
        )
        .WithPlayer(
            new PlayerBuilder(B.Id)
            .WithMoney(B.Money)
            .Build()
        )
        .WithPlayer(
            new PlayerBuilder(C.Id)
            .WithMoney(C.Money)
            .Build()
        )
        .WithMockDice(new[] { 1 })
        .WithCurrentPlayer(new CurrentPlayerStateBuilder(A.Id).Build());

        monopolyBuilder.Save(server);

        var hub = await server.CreateHubConnectionAsync(gameId, "A");

        // Act
        await hub.SendAsync(nameof(MonopolyHub.Settlement), gameId, "A");

        // Assert
        // 遊戲結算
        hub.Verify<string, int>(
                       nameof(IMonopolyResponses.SettlementEvent),
                                  (playerId, rank)
                                  => playerId == "C" && rank == 3);
        hub.Verify<string, int>(
                       nameof(IMonopolyResponses.SettlementEvent),
                                  (playerId, rank)
                                  => playerId == "A" && rank == 2);
        hub.Verify<string, int>(
                       nameof(IMonopolyResponses.SettlementEvent),
                                  (playerId, rank)
                                  => playerId == "B" && rank == 1);
        
        hub.VerifyNoElseEvent();
    }
}