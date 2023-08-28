using Domain;
using Server.Hubs;
using SharedLibrary;
using static Domain.Map;
using static ServerTests.Utils;

namespace ServerTests.AcceptanceTests;

[TestClass]
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
        Player A = new("A", 1000);
        Player B = new("B", 0);
        Player C = new("C", 0);

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
            .WithBankrupt()
        )
        .WithPlayer(
            new MonopolyPlayer(C.Id)
            .WithMoney(C.Money)
            .WithPosition("A1", Direction.Right.ToString())
            .WithBankrupt()
        )
        .WithCurrentPlayer(nameof(A));

        monopolyBuilder.Save(server);

        var hub = await server.CreateHubConnectionAsync(gameId, A.Id);

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
        Player A = new("A", 800);
        Player B = new("B", 1000);
        Player C = new("C", 600);

        const string gameId = "1";
        var monopolyBuilder = new MonopolyBuilder("1")
        .WithPlayer(
            new MonopolyPlayer(A.Id)
            .WithMoney(A.Money)
            .WithPosition("A1", Direction.Right.ToString())
            .WithLandContract("A2")
        )
        .WithPlayer(
            new MonopolyPlayer(B.Id)
            .WithMoney(B.Money)
            .WithPosition("A1", Direction.Right.ToString())
        )
        .WithPlayer(
            new MonopolyPlayer(C.Id)
            .WithMoney(C.Money)
            .WithPosition("A1", Direction.Right.ToString())
        )
        .WithCurrentPlayer(nameof(A));

        monopolyBuilder.Save(server);

        var hub = await server.CreateHubConnectionAsync(gameId, A.Id);

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
        Player A = new("A", 800);
        Player B = new("B", 1000);
        Player C = new("C", 600);

        const string gameId = "1";
        var monopolyBuilder = new MonopolyBuilder("1")
        .WithPlayer(
            new MonopolyPlayer(A.Id)
            .WithMoney(A.Money)
            .WithPosition("A1", Direction.Right.ToString())
            .WithLandContract("A2")
            .WithMortgage("A2")

        )
        .WithPlayer(
            new MonopolyPlayer(B.Id)
            .WithMoney(B.Money)
            .WithPosition("A1", Direction.Right.ToString())
        )
        .WithPlayer(
            new MonopolyPlayer(C.Id)
            .WithMoney(C.Money)
            .WithPosition("A1", Direction.Right.ToString())
        )
        .WithCurrentPlayer(nameof(A));

        monopolyBuilder.Save(server);

        var hub = await server.CreateHubConnectionAsync(gameId, A.Id);

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