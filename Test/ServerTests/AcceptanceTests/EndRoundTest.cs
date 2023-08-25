using Domain;
using Server.Hubs;
using SharedLibrary;
using static Domain.Map;
using static ServerTests.Utils;

namespace ServerTests.AcceptanceTests;

[TestClass]
public class EndRoundTest
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
        Given:  目前輪到 A
                A 持有 1000元
                B 持有 1000元
                A2 是 B 的土地，價值 1000元
                A 移動到 A2
        When:   A 結束回合
        Then:   A 無法結束回合
        """)]
    public async Task 玩家沒有付過路費無法結束回合()
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
            .WithLandContract("A2")
        )
        .WithMockDice(new[] { 1, 1 })
        .WithCurrentPlayer(nameof(A), rollDice : true);

        monopolyBuilder.Save(server);

        var hub = await server.CreateHubConnectionAsync(gameId, A.Id);

        // Act
        await hub.SendAsync(nameof(MonopolyHub.EndRound), gameId, "A");

        // Assert
        // A 擲了 2 點
        // A 移動到 Station1，方向為 Right，剩下 1 步
        // A 移動到 A2，方向為 Right，剩下 0 步
        // A 需要支付過路費
        // A 結束回合
        hub.Verify<string, int>(
            nameof(IMonopolyResponses.PlayerRolledDiceEvent),
            (playerId, diceCount) => playerId == "A" && diceCount == 2);
        VerifyChessMovedEvent(hub, "A", "Station1", "Right", 1);
        VerifyChessMovedEvent(hub, "A", "A2", "Right", 0);
        hub.Verify<string, string, decimal>(
                       nameof(IMonopolyResponses.PlayerNeedsToPayTollEvent),
                                  (playerId, ownId, toll) => playerId == "A" && ownId == "B" && toll == 50);
        hub.Verify<string>(
                       nameof(IMonopolyResponses.EndRoundFailEvent),
                                  (playerId)
                                  => playerId == "A");
        hub.VerifyNoElseEvent();
    }

    [TestMethod]
    [Description(
        """
        Given:  目前輪到 A
                A 持有 1000元
                B 持有 1000元
                A2 是 B 的土地，價值 1000元
                A 移動到 A2
                A 支付過路費
        When:   A 結束回合
        Then:   A 結束回合, 輪到玩家 B 的回合
        """)]
    public async Task 玩家成功結束回合()
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
            .WithLandContract("A2")
        )
        .WithMockDice(new[] { 1, 1 })
        .WithCurrentPlayer(nameof(A), rollDice : true, payToll: true);

        monopolyBuilder.Save(server);

        var hub = await server.CreateHubConnectionAsync(gameId, A.Id);

        // Act
        await hub.SendAsync(nameof(MonopolyHub.EndRound), gameId, "A");

        // Assert
        // A 擲了 2 點
        // A 移動到 Station1，方向為 Right，剩下 1 步
        // A 移動到 A2，方向為 Right，剩下 0 步
        // A 需要支付過路費
        // A 支付過路費
        // A 結束回合
        hub.Verify<string, int>(
            nameof(IMonopolyResponses.PlayerRolledDiceEvent),
            (playerId, diceCount) => playerId == "A" && diceCount == 2);
        VerifyChessMovedEvent(hub, "A", "Station1", "Right", 1);
        VerifyChessMovedEvent(hub, "A", "A2", "Right", 0);
        hub.Verify<string, string, decimal>(
                       nameof(IMonopolyResponses.PlayerNeedsToPayTollEvent),
                                  (playerId, ownId, toll) => playerId == "A" && ownId == "B" && toll == 50);
        hub.Verify<string, decimal, string, decimal>(
                       nameof(IMonopolyResponses.PlayerPayTollEvent),
                                  (playerId, playerMoney, ownerId, ownerMoney)
                                  => playerId == "A" && playerMoney == 950 && ownerId == "B" && ownerMoney == 1050);                         
        hub.Verify<string, string>(
                       nameof(IMonopolyResponses.EndRoundEvent),
                                  (playerId, nextPlayer)
                                  => playerId == "A" && nextPlayer == "B");
        hub.VerifyNoElseEvent();
    }

    [TestMethod]
    [Description(
        """
        Given:  目前輪到 A
                A 持有 1000元
                A 抵押 A1 剩餘 1回合
                B 持有 1000元
                A2 是 B 的土地，價值 1000元
                A 移動到 A2
                A 支付過路費
        When:   A 結束回合
        Then:   A 結束回合, 輪到玩家 B 的回合，A1 抵押到期為系統所有
        """)]
    public async Task 玩家10回合後沒贖回房地產失去房地產()
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
            .WithLandContract("A1", 2)
            .WithMortgage("A1", 1)
        )
        .WithPlayer(
            new MonopolyPlayer(B.Id)
            .WithMoney(B.Money)
            .WithPosition("A1", Direction.Right.ToString())
            .WithLandContract("A2")
        )
        .WithMockDice(new[] { 1, 1 })
        .WithCurrentPlayer(nameof(A), rollDice : true, payToll: true);

        monopolyBuilder.Save(server);

        var hub = await server.CreateHubConnectionAsync(gameId, A.Id);

        // Act
        await hub.SendAsync(nameof(MonopolyHub.EndRound), gameId, "A");

        // Assert
        // A 擲了 2 點
        // A 移動到 Station1，方向為 Right，剩下 1 步
        // A 移動到 A2，方向為 Right，剩下 0 步
        // A 需要支付過路費
        // A 支付過路費
        // A 結束回合
        // A1 抵押到期
        hub.Verify<string, int>(
            nameof(IMonopolyResponses.PlayerRolledDiceEvent),
            (playerId, diceCount) => playerId == "A" && diceCount == 2);
        VerifyChessMovedEvent(hub, "A", "Station1", "Right", 1);
        VerifyChessMovedEvent(hub, "A", "A2", "Right", 0);
        hub.Verify<string, string, decimal>(
                       nameof(IMonopolyResponses.PlayerNeedsToPayTollEvent),
                                  (playerId, ownId, toll) => playerId == "A" && ownId == "B" && toll == 50);
        hub.Verify<string, decimal, string, decimal>(
                       nameof(IMonopolyResponses.PlayerPayTollEvent),
                                  (playerId, playerMoney, ownerId, ownerMoney)
                                  => playerId == "A" && playerMoney == 950 && ownerId == "B" && ownerMoney == 1050);                         
        hub.Verify<string, string>(
                       nameof(IMonopolyResponses.EndRoundEvent),
                                  (playerId, nextPlayer)
                                  => playerId == "A" && nextPlayer == "B");
        hub.Verify<string, string>(
                       nameof(IMonopolyResponses.MortgageDueEvent),
                                  (playerId, BlockId)
                                  => playerId == "A" && BlockId == "A1");
        hub.VerifyNoElseEvent();
    }
}