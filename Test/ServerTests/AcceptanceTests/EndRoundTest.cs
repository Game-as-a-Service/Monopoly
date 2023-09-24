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
                還沒付過路費
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
        // A 結束回合
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
        // A 結束回合                      
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
        var A = new { Id = "A", Money = 1000m };
        var B = new { Id = "B", Money = 1000m };

        const string gameId = "1";
        var monopolyBuilder = new MonopolyBuilder("1")
        .WithPlayer(
            new PlayerBuilder(A.Id)
            .WithMoney(A.Money)
            .WithPosition("A1", Direction.Right)
            .WithLandContract("A1", true, 1)
            .Build()
        )
        .WithPlayer(
            new PlayerBuilder(B.Id)
            .WithMoney(B.Money)
            .WithPosition("A1", Direction.Right)
            .WithLandContract("A2")
            .Build()
        )
        .WithMockDice(new[] { 1, 1 })
        .WithCurrentPlayer(new CurrentPlayerStateBuilder(A.Id).WithPayToll().Build());

        monopolyBuilder.Save(server);

        var hub = await server.CreateHubConnectionAsync(gameId, A.Id);

        // Act
        await hub.SendAsync(nameof(MonopolyHub.EndRound), gameId, "A");

        // Assert
        // A 結束回合
        // A1 抵押到期
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
        var A = new { Id = "A", Money = 1000m };
        var B = new { Id = "B", Money = 0m };
        var C = new { Id = "C", Money = 1000m };

        const string gameId = "1";
        var monopolyBuilder = new MonopolyBuilder("1")
        .WithPlayer(
            new PlayerBuilder(A.Id)
            .WithMoney(A.Money)
            .WithPosition("A1", Direction.Right)
            .Build()
        )
        .WithPlayer(
            new PlayerBuilder(B.Id)
            .WithMoney(B.Money)
            .WithPosition("A1", Direction.Right)
            .WithBankrupt(5)
            .Build()
        )
        .WithPlayer(
            new PlayerBuilder(C.Id)
            .WithMoney(C.Money)
            .WithPosition("A1", Direction.Right)
            .WithLandContract("A2")
            .Build()
        )
        .WithMockDice(new[] { 1, 1 })
        .WithCurrentPlayer(new CurrentPlayerStateBuilder(A.Id).WithPayToll().Build());

        monopolyBuilder.Save(server);

        var hub = await server.CreateHubConnectionAsync(gameId, A.Id);

        // Act
        await hub.SendAsync(nameof(MonopolyHub.EndRound), gameId, "A");

        // Assert
        // A 結束回合，輪到下一個未破產玩家
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
        var A = new { Id = "A", Money = 1000m };
        var B = new { Id = "B", Money = 1000m };
        var C = new { Id = "C", Money = 1000m };

        const string gameId = "1";
        var monopolyBuilder = new MonopolyBuilder("1")
        .WithPlayer(
            new PlayerBuilder(A.Id)
            .WithMoney(A.Money)
            .WithPosition("A1", Direction.Right)
            .Build()
        )
        .WithPlayer(
            new PlayerBuilder(B.Id)
            .WithMoney(B.Money)
            .WithPosition("Jail", Direction.Right)
            .Build()
        )
        .WithPlayer(
            new PlayerBuilder(C.Id)
            .WithMoney(C.Money)
            .WithPosition("A1", Direction.Right)
            .Build()
        )
        .WithMockDice(new[] { 1, 1 })
        .WithCurrentPlayer(new CurrentPlayerStateBuilder(A.Id).Build());

        monopolyBuilder.Save(server);

        var hub = await server.CreateHubConnectionAsync(gameId, A.Id);

        // Act
        await hub.SendAsync(nameof(MonopolyHub.EndRound), gameId, "A");

        // Assert
        // A 結束回合，輪到下一個玩家 B
        // B 在監獄，輪到下一個玩家 C
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