using Domain.Events;
using Server.Hubs;
using SharedLibrary;
using SharedLibrary.ResponseArgs.Monopoly;
using static ServerTests.Utils;

namespace ServerTests.AcceptanceTests;

[TestClass]
public class RollDiceTest
{
    private MonopolyTestServer _server = default!;

    [TestInitialize]
    public void Setup()
    {
        _server = new MonopolyTestServer();
    }

    [TestMethod]
    [Description(
        """
        Given:  目前玩家在F4
        When:   玩家擲骰得到6點
        Then:   A 移動到 A4
        """)]
    public async Task 玩家擲骰後移動棋子()
    {
        // Arrange
        var a = new { Id = "A" };

        const string gameId = "1";
        var monopolyBuilder = new MonopolyBuilder("1")
        .WithPlayer(
            new PlayerBuilder(a.Id)
            .WithPosition("F4", Direction.Up)
            .Build()
        )
        .WithMockDice([6])
        .WithCurrentPlayer(new CurrentPlayerStateBuilder(a.Id).Build());

        monopolyBuilder.Save(_server);

        var hub = await _server.CreateHubConnectionAsync(gameId, "A");

        // Act
        await hub.SendAsync(nameof(MonopolyHub.PlayerRollDice), gameId, "A");

        // Assert
        // A 擲了 6 點
        // A 移動到 Start，方向為 Right，剩下 5 步
        // A 移動到 A1，方向為 Right，剩下 4 步
        // A 移動到 Station1，方向為 Right，剩下 3 步
        // A 移動到 A2，方向為 Right，剩下 2 步
        // A 移動到 A3，方向為 Down，剩下 1 步
        // A 移動到 A4，方向為 Down，剩下 0 步
        hub.Verify(nameof(IMonopolyResponses.PlayerRolledDiceEvent),
            (PlayerRolledDiceEventArgs e) => e is { PlayerId: "A", DiceCount: 6 });
        VerifyChessMovedEvent(hub, "A", "Start", "Right", 5);
        VerifyChessMovedEvent(hub, "A", "A1", "Right", 4);
        VerifyChessMovedEvent(hub, "A", "Station1", "Right", 3);
        VerifyChessMovedEvent(hub, "A", "A2", "Right", 2);
        VerifyChessMovedEvent(hub, "A", "A3", "Down", 1);
        VerifyChessMovedEvent(hub, "A", "A4", "Down", 0);
    }

    [TestMethod]
    [Description(
        """
        Given:  目前玩家在F4
        When:   玩家擲骰得到8點
        Then:   A 移動到 停車場
                玩家需要選擇方向
                玩家剩餘步數為 1
        """)]
    public async Task 玩家擲骰後移動棋子到需要選擇方向的地方()
    {
        // Arrange
        var A = new { Id = "A" };
        string[] expectedDirections = [ "Right", "Down", "Left" ];
        const string gameId = "1";
        var monopolyBuilder = new MonopolyBuilder("1")
        .WithPlayer(
            new PlayerBuilder(A.Id)
            .WithPosition("F4", Direction.Up)
            .Build()
        )
        .WithMockDice([2, 6])
        .WithCurrentPlayer(new CurrentPlayerStateBuilder(A.Id).Build());

        monopolyBuilder.Save(_server);

        var hub = await _server.CreateHubConnectionAsync(gameId, "A");

        // Act
        await hub.SendAsync(nameof(MonopolyHub.PlayerRollDice), gameId, "A");

        //Assert
        // A 擲了 8 點
        // A 移動到 Start，方向為 Right，剩下 7 步
        // A 移動到 A1，方向為 Right，剩下 6 步
        // A 移動到 Station1，方向為 Right，剩下 5 步
        // A 移動到 A2，方向為 Right，剩下 4 步
        // A 移動到 A3，方向為 Down，剩下 3 步
        // A 移動到 A4，方向為 Down，剩下 2 步
        // A 移動到 ParkingLot，方向為 Down，剩下 1 步
        // A 需要選擇方向，可選擇的方向為 Right, Down, Left
        hub.Verify(nameof(IMonopolyResponses.PlayerRolledDiceEvent),
            (PlayerRolledDiceEventArgs e) => e is { PlayerId: "A", DiceCount: 8 });
        VerifyChessMovedEvent(hub, "A", "Start", "Right", 7);
        VerifyChessMovedEvent(hub, "A", "A1", "Right", 6);
        hub.Verify(
            nameof(IMonopolyResponses.ThroughStartEvent),
            (PlayerThroughStartEventArgs e) => e is { PlayerId: "A", GainMoney: 3000, TotalMoney: 18000 });
        VerifyChessMovedEvent(hub, "A", "Station1", "Right", 5);
        VerifyChessMovedEvent(hub, "A", "A2", "Right", 4);
        VerifyChessMovedEvent(hub, "A", "A3", "Down", 3);
        VerifyChessMovedEvent(hub, "A", "A4", "Down", 2);
        //VerifyChessMovedEvent(hub, "A", "ParkingLot", "Down", 1);
        hub.Verify(
            nameof(IMonopolyResponses.PlayerNeedToChooseDirectionEvent),
            (PlayerNeedToChooseDirectionEventArgs e) =>
            {
                return e.PlayerId == "A" && e.Directions.OrderBy(x => x)
                    .SequenceEqual(expectedDirections.OrderBy(x => x));
            });
        hub.VerifyNoElseEvent();
    }

    [TestMethod]
    [Description(
        """
        Given
            目前玩家A在F3
            玩家持有1000元
        When
            玩家擲骰得到4點
        Then
            玩家移動到 A1
            玩家剩餘步數為 0
            玩家持有4000元
        """)]
    public async Task 玩家擲骰後移動棋子經過起點獲得獎勵金3000()
    {
        // Arrange
        var A = new { Id = "A", Money = 1000m };

        const string gameId = "1";
        var monopolyBuilder = new MonopolyBuilder("1")
        .WithPlayer(
            new PlayerBuilder(A.Id)
            .WithMoney(A.Money)
            .WithPosition("F3", Direction.Up)
            .Build()
        )
        .WithMockDice(new[] { 2, 2 })
        .WithCurrentPlayer(new CurrentPlayerStateBuilder(A.Id).Build());

        monopolyBuilder.Save(_server);

        var hub = await _server.CreateHubConnectionAsync(gameId, "A");

        // Act
        await hub.SendAsync(nameof(MonopolyHub.PlayerRollDice), "1", "A");

        // Assert
        // A 擲了 4 點
        // A 移動到 Station4，方向為 Up，剩下 3 步
        // A 移動到 F4，方向為 Up，剩下 2 步
        // A 移動到 Start，方向為 Right，剩下 1 步
        // A 獲得獎勵金3000，共持有4000元
        // A 移動到 A1，方向為 Right，剩下 0 步
        hub.Verify(
            nameof(IMonopolyResponses.PlayerRolledDiceEvent),
            (PlayerRolledDiceEventArgs e) => e is { PlayerId: "A", DiceCount: 4 });
        VerifyChessMovedEvent(hub, "A", "Station4", "Up", 3);
        VerifyChessMovedEvent(hub, "A", "F4", "Up", 2);
        VerifyChessMovedEvent(hub, "A", "Start", "Right", 1);
        VerifyChessMovedEvent(hub, "A", "A1", "Right", 0);
        hub.Verify(nameof(IMonopolyResponses.ThroughStartEvent),
            (PlayerThroughStartEventArgs e) => e is { PlayerId: "A", GainMoney: 3000, TotalMoney: 4000 });
        hub.Verify(nameof(IMonopolyResponses.PlayerCanBuyLandEvent),
            (PlayerCanBuyLandEventArgs e) => e is { PlayerId: "A", LandId: "A1", Price: 1000 });
        hub.VerifyNoElseEvent();
    }

    [TestMethod]
    [Description(
        """
        Given:  目前玩家在F3
                玩家持有1000元
        When:   玩家擲骰得到3點
        Then:   玩家移動到 起點
                玩家剩餘步數為 0
                玩家持有1000元
        """)]
    public async Task 玩家擲骰後移動棋子到起點無法獲得獎勵金()
    {
        // Arrange
        var A = new { Id = "A", Money = 1000m };

        const string gameId = "1";
        var monopolyBuilder = new MonopolyBuilder("1")
        .WithPlayer(
            new PlayerBuilder(A.Id)
            .WithMoney(A.Money)
            .WithPosition("F3", Direction.Up)
            .Build()
        )
        .WithMockDice(new[] { 2, 1 })
        .WithCurrentPlayer(new CurrentPlayerStateBuilder(A.Id).Build());

        monopolyBuilder.Save(_server);

        var hub = await _server.CreateHubConnectionAsync(gameId, "A");

        // Act
        await hub.SendAsync(nameof(MonopolyHub.PlayerRollDice), "1", "A");

        // Assert
        // A 擲了 3 點
        // A 移動到 Station4，方向為 Up，剩下 2 步
        // A 移動到 F4，方向為 Up，剩下 1 步
        // A 移動到 Start，方向為 Right，剩下 0 步
        // A 沒有獲得獎勵金，共持有1000元
        hub.Verify(nameof(IMonopolyResponses.PlayerRolledDiceEvent),
            (PlayerRolledDiceEventArgs e) => e is { PlayerId: "A", DiceCount: 3 });
        VerifyChessMovedEvent(hub, "A", "Station4", "Up", 2);
        VerifyChessMovedEvent(hub, "A", "F4", "Up", 1);
        VerifyChessMovedEvent(hub, "A", "Start", "Right", 0);
        hub.Verify(
            nameof(IMonopolyResponses.CannotGetRewardBecauseStandOnStartEvent),
            (CannotGetRewardBecauseStandOnStartEventArgs e) => e.PlayerId == "A");
        hub.VerifyNoElseEvent();
    }

    [TestMethod]
    [Description("""
                Given:  目前玩家在A1
                        玩家持有A2
                        A2房子不足5間
                When:   玩家擲骰得到2點
                Then:   玩家移動到 A2
                        玩家剩餘步數為 0
                        提示可以蓋房子
                """)]
    public async Task 玩家擲骰後移動棋子到自己擁有地()
    {
        // Arrange
        var A = new { Id = "A" };
        var A1 = new { Id = "A1" };
        var A2 = new { Id = "A2", House = 4 };

        const string gameId = "1";
        var monopolyBuilder = new MonopolyBuilder("1")
        .WithPlayer(
            new PlayerBuilder(A.Id)
            .WithPosition(A1.Id, Direction.Right)
            .WithLandContract(A2.Id)
            .Build()
        )
        .WithMockDice([1, 1])
        .WithCurrentPlayer(new CurrentPlayerStateBuilder(A.Id).Build())
        .WithLandHouse(A2.Id, A2.House);

        monopolyBuilder.Save(_server);

        var hub = await _server.CreateHubConnectionAsync(gameId, "A");
        // Act
        await hub.SendAsync(nameof(MonopolyHub.PlayerRollDice), "1", "A");
        // Assert
        // A 擲了 2 點
        // A 移動到 A2，方向為 Right，剩下 0 步
        // A 可以蓋房子
        hub.Verify(nameof(IMonopolyResponses.PlayerRolledDiceEvent),
                                  (PlayerRolledDiceEventArgs e) => e is { PlayerId: "A", DiceCount: 2 });
        VerifyChessMovedEvent(hub, "A", "Station1", "Right", 1);
        VerifyChessMovedEvent(hub, "A", "A2", "Right", 0);
        hub.Verify(nameof(IMonopolyResponses.PlayerCanBuildHouseEvent),
             (PlayerCanBuildHouseEventArgs e) => e is { PlayerId: "A", LandId: "A2", HouseCount: 4, Price: 1000 });
        hub.VerifyNoElseEvent();
    }

    [TestMethod]
    [Description("""
                Given
                    目前玩家A在A1
                    玩家B持有A2(無房子)
                When
                    玩家擲骰得到2點
                Then
                    玩家A移動到 A2
                    玩家剩餘步數為 0
                    提示需要付過路費
                """)]
    public async Task 玩家擲骰後移動棋子到他人擁有地()
    {
        // Arrange
        var A = new { Id = "A" };
        var B = new { Id = "B" };
        var A2 = new { Id = "A2", House = 0 };

        const string gameId = "1";
        var monopolyBuilder = new MonopolyBuilder("1")
        .WithPlayer(
            new PlayerBuilder(A.Id)
            .WithPosition("A1", Direction.Right)
            .Build()
        )
        .WithPlayer(
            new PlayerBuilder(B.Id)
            .WithLandContract(A2.Id)
            .Build()
        )
        .WithMockDice([2])
        .WithCurrentPlayer(new CurrentPlayerStateBuilder(A.Id).Build());

        monopolyBuilder.Save(_server);

        var hub = await _server.CreateHubConnectionAsync(gameId, "A");
        // Act
        await hub.SendAsync(nameof(MonopolyHub.PlayerRollDice), "1", "A");
        // Assert
        // A 擲了 2 點
        // A 移動到 A2，方向為 Right，剩下 0 步
        // A 需要支付過路費
        hub.Verify(nameof(IMonopolyResponses.PlayerRolledDiceEvent),
                (PlayerRolledDiceEventArgs e) => e is { PlayerId: "A", DiceCount: 2 });
        VerifyChessMovedEvent(hub, "A", "Station1", "Right", 1);
        VerifyChessMovedEvent(hub, "A", "A2", "Right", 0);
        hub.Verify(nameof(IMonopolyResponses.PlayerNeedsToPayTollEvent),
                                  (PlayerNeedsToPayTollEventArgs e) => e is { PlayerId: "A", OwnerId: "B", Toll: 50 });
        hub.VerifyNoElseEvent();
    }

    [TestMethod]
    [Description("""
                Given:  目前玩家在A1
                When:   玩家擲骰得到2點
                Then:   玩家移動到 A2
                        玩家剩餘步數為 0
                        提示可以購買空地
                """)]
    public async Task 玩家擲骰後移動棋子到空地()
    {
        // Arrange
        var A = new { Id = "A" };

        const string gameId = "1";
        var monopolyBuilder = new MonopolyBuilder("1")
        .WithPlayer(
            new PlayerBuilder(A.Id)
            .WithPosition("A1", Direction.Right)
            .Build()
        )
        .WithMockDice(new[] { 2 })
        .WithCurrentPlayer(new CurrentPlayerStateBuilder(A.Id).Build());

        monopolyBuilder.Save(_server);

        var hub = await _server.CreateHubConnectionAsync(gameId, "A");
        // Act
        await hub.SendAsync(nameof(MonopolyHub.PlayerRollDice), "1", "A");
        // Assert
        // A 擲了 2 點
        // A 移動到 A2，方向為 Right，剩下 0 步
        // A 可以購買空地
        hub.Verify(nameof(IMonopolyResponses.PlayerRolledDiceEvent),
                                  (PlayerRolledDiceEventArgs e) => e is { PlayerId: "A", DiceCount: 2 });
        VerifyChessMovedEvent(hub, "A", "Station1", "Right", 1);
        VerifyChessMovedEvent(hub, "A", "A2", "Right", 0);
        hub.Verify(nameof(IMonopolyResponses.PlayerCanBuyLandEvent),
                                  (PlayerCanBuyLandEventArgs e) => e is { PlayerId:"A", LandId: "A2", Price: 1000 });
        hub.VerifyNoElseEvent();
    }
}