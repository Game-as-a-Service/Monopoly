using Server.Hubs;
using SharedLibrary;
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
        Given:  目前輪到 A 目前在 A2
                A 持有 1000元
                B 持有 1000元
                A2 是 B 的土地，價值 1000元
        When:   A 結束回合
        Then:   A 無法結束回合
        """)]
    public async Task 玩家沒有付過路費無法結束回合()
    {
        // Arrange
        var A = new { Id = "A", Money = 1000m };
        var B = new { Id = "B", Money = 1000m };
        var A2 = new { Id = "A2", Price = 1000m };

        const string gameId = "1";
        var monopolyBuilder = new MonopolyBuilder("1")
        .WithPlayer(
            new PlayerBuilder(A.Id)
            .WithMoney(A.Money)
            .WithPosition(A2.Id, Direction.Left)
            .Build()
        )
        .WithPlayer(
            new PlayerBuilder(B.Id)
            .WithMoney(B.Money)
            .WithLandContract(A2.Id)
            .Build()
        )
        .WithCurrentPlayer(new CurrentPlayerStateBuilder(A.Id)
            .Build()
        );

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
                A 已支付過路費
        When:   A 結束回合
        Then:   A 結束回合, 輪到玩家 B 的回合
        """)]
    public async Task 玩家成功結束回合()
    {
        // Arrange
        var A = new { Id = "A", Money = 1000m };
        var B = new { Id = "B", Money = 1000m };
        var A2 = new { Id = "A2", Price = 1000m };

        const string gameId = "1";
        var monopolyBuilder = new MonopolyBuilder("1")
        .WithPlayer(
            new PlayerBuilder(A.Id)
            .WithMoney(A.Money)
            .WithPosition(A2.Id, Direction.Up)
            .Build()
        )
        .WithPlayer(
            new PlayerBuilder(B.Id)
            .WithMoney(B.Money)
            .WithLandContract(A2.Id)
            .Build()
        )
        .WithCurrentPlayer(new CurrentPlayerStateBuilder(A.Id)
            .WithPayToll()
            .Build()
        );

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
            new PlayerBuilder(A.Id)
            .WithMoney(A.Money)
            .WithPosition("A1", Direction.Right.ToString())
            .WithLandContract("A1", 2)
            .WithMortgage("A1", 1)
        )
        .WithPlayer(
            new PlayerBuilder(B.Id)
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

    [TestMethod]
    [Description(
        """
        Given:  目前輪到 A
                A 持有 1000元
                B 持有 0元，狀態為破產
                C 持有 1000元
                A2 是 C 的土地，價值 1000元
                A 移動到 A2
                A 支付過路費
        When:   A 結束回合
        Then:   A 結束回合, 輪到下一個未破產玩家 C
        """)]
    public async Task 結束回合輪到下一個未破產玩家()
    {
        // Arrange
        Player A = new("A", 1000);
        Player B = new("B", 0);
        Player C = new("C", 1000);

        const string gameId = "1";
        var monopolyBuilder = new MonopolyBuilder("1")
        .WithPlayer(
            new PlayerBuilder(A.Id)
            .WithMoney(A.Money)
            .WithPosition("A1", Direction.Right.ToString())
        )
        .WithPlayer(
            new PlayerBuilder(B.Id)
            .WithMoney(B.Money)
            .WithPosition("A1", Direction.Right.ToString())
            .WithBankrupt()
        )
        .WithPlayer(
            new PlayerBuilder(C.Id)
            .WithMoney(C.Money)
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
        // A 結束回合，輪到下一個未破產玩家
        hub.Verify<string, int>(
            nameof(IMonopolyResponses.PlayerRolledDiceEvent),
            (playerId, diceCount) => playerId == "A" && diceCount == 2);
        VerifyChessMovedEvent(hub, "A", "Station1", "Right", 1);
        VerifyChessMovedEvent(hub, "A", "A2", "Right", 0);
        hub.Verify<string, string, decimal>(
                       nameof(IMonopolyResponses.PlayerNeedsToPayTollEvent),
                                  (playerId, ownId, toll) => playerId == "A" && ownId == "C" && toll == 50);
        hub.Verify<string, decimal, string, decimal>(
                       nameof(IMonopolyResponses.PlayerPayTollEvent),
                                  (playerId, playerMoney, ownerId, ownerMoney)
                                  => playerId == "A" && playerMoney == 950 && ownerId == "C" && ownerMoney == 1050);                         
        hub.Verify<string, string>(
                       nameof(IMonopolyResponses.EndRoundEvent),
                                  (playerId, nextPlayer)
                                  => playerId == "A" && nextPlayer == "C");
        hub.VerifyNoElseEvent();
    }

    [TestMethod]
    [Description(
        """
        Given:  目前輪到 A
                A 持有 1000元
                B 持有 1000元，上一回合到監獄
                C 持有 1000元
                A 移動到 A2
        When:   A 結束回合
        Then:   A 結束回合，輪到下一個玩家 B
                B 在監獄，輪到下一個玩家 C
        """)]
    public async Task 上一回合玩家剛到監獄這回合不能做任何事()
    {
        // Arrange
        Player A = new("A", 1000);
        Player B = new("B", 1000);
        Player C = new("C", 1000);

        const string gameId = "1";
        var monopolyBuilder = new MonopolyBuilder("1")
        .WithPlayer(
            new PlayerBuilder(A.Id)
            .WithMoney(A.Money)
            .WithPosition("A1", Direction.Right.ToString())
        )
        .WithPlayer(
            new PlayerBuilder(B.Id)
            .WithMoney(B.Money)
            .WithPosition("Jail", Direction.Right.ToString())
        )
        .WithPlayer(
            new PlayerBuilder(C.Id)
            .WithMoney(C.Money)
            .WithPosition("A1", Direction.Right.ToString())
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
        // A 結束回合，輪到下一個玩家 B
        // B 在監獄，輪到下一個玩家 C
        hub.Verify<string, int>(
            nameof(IMonopolyResponses.PlayerRolledDiceEvent),
            (playerId, diceCount) => playerId == "A" && diceCount == 2);
        VerifyChessMovedEvent(hub, "A", "Station1", "Right", 1);
        VerifyChessMovedEvent(hub, "A", "A2", "Right", 0);
        hub.Verify<string, string, decimal>(
                       nameof(IMonopolyResponses.PlayerCanBuyLandEvent),
                                  (playerId, blockId, landMoney) => playerId == "A" && blockId == "A2" && landMoney == 1000);
        hub.Verify<string, int>(
                       nameof(IMonopolyResponses.SuspendRoundEvent),
                                  (playerId, suspendRounds)
                                  => playerId == "B" && suspendRounds == 1);
        hub.Verify<string, string>(
                       nameof(IMonopolyResponses.EndRoundEvent),
                                  (playerId, nextPlayer)
                                  => playerId == "A" && nextPlayer == "B");
        hub.Verify<string, string>(
                       nameof(IMonopolyResponses.EndRoundEvent),
                                  (playerId, nextPlayer)
                                  => playerId == "B" && nextPlayer == "C");
                                  
        hub.VerifyNoElseEvent();
    }
}