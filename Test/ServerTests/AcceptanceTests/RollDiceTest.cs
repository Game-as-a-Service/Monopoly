using Application.Common;
using Domain;
using Domain.Maps;
using Server.Hubs;
using static Domain.Map;
using static ServerTests.Utils;

namespace ServerTests.AcceptanceTests;

[TestClass]
public class RollDiceTest
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
        Given:  目前玩家在F4
        When:   玩家擲骰得到6點
        Then:   A 移動到 A4
        """)]
    public async Task 玩家擲骰後移動棋子()
    {
        // Arrange
        Player A = new("A");

        var monopolyBuilder = new MonopolyBuilder("1")
        .WithPlayer(
            new MonopolyPlayer(A.Id)
            .WithMoney(A.Money)
            .WithPosition("F4", Direction.Up.ToString())
        )
        .WithMockDice(new[] { 6 })
        .WithCurrentPlayer(nameof(A));

        monopolyBuilder.Save(server);

        var hub = server.CreateHubConnection();

        // Act
        await hub.SendAsync(nameof(MonopolyHub.PlayerRollDice), "1", "A");

        // Assert
        // A 擲了 6 點
        // A 移動到 Start，方向為 Right，剩下 5 步
        // A 移動到 A1，方向為 Right，剩下 4 步
        // A 移動到 Station1，方向為 Right，剩下 3 步
        // A 移動到 A2，方向為 Right，剩下 2 步
        // A 移動到 A3，方向為 Down，剩下 1 步
        // A 移動到 A4，方向為 Down，剩下 0 步
        hub.Verify<string, int>(
            nameof(IMonopolyResponses.PlayerRolledDiceEvent),
            (playerId, diceCount) => playerId == "A" && diceCount == 6);
        Utils.VerifyChessMovedEvent(hub, "A", "Start", "Right", 5);
        Utils.VerifyChessMovedEvent(hub, "A", "A1", "Right", 4);
        Utils.VerifyChessMovedEvent(hub, "A", "Station1", "Right", 3);
        Utils.VerifyChessMovedEvent(hub, "A", "A2", "Right", 2);
        Utils.VerifyChessMovedEvent(hub, "A", "A3", "Down", 1);
        Utils.VerifyChessMovedEvent(hub, "A", "A4", "Down", 0);
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
        Player A = new("A");

        var monopolyBuilder = new MonopolyBuilder("1")
        .WithPlayer(
            new MonopolyPlayer(A.Id)
            .WithMoney(A.Money)
            .WithPosition("F4", Direction.Up.ToString())
        )
        .WithMockDice(new[] { 2, 6 })
        .WithCurrentPlayer(nameof(A));

        monopolyBuilder.Save(server);

        var hub = server.CreateHubConnection();

        // Act
        await hub.SendAsync(nameof(MonopolyHub.PlayerRollDice), "1", "A");

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
        hub.Verify<string, int>(
            nameof(IMonopolyResponses.PlayerRolledDiceEvent),
            (playerId, diceCount) => playerId == "A" && diceCount == 8);
        Utils.VerifyChessMovedEvent(hub, "A", "Start", "Right", 7);
        Utils.VerifyChessMovedEvent(hub, "A", "A1", "Right", 6);
        hub.Verify<string, int, decimal>(
            nameof(IMonopolyResponses.ThroughStartEvent),
            (playerId, gainMoney, totalMoney) => playerId == "A" && gainMoney == 3000 && totalMoney == 18000);
        Utils.VerifyChessMovedEvent(hub, "A", "Station1", "Right", 5);
        Utils.VerifyChessMovedEvent(hub, "A", "A2", "Right", 4);
        Utils.VerifyChessMovedEvent(hub, "A", "A3", "Down", 3);
        Utils.VerifyChessMovedEvent(hub, "A", "A4", "Down", 2);
        Utils.VerifyChessMovedEvent(hub, "A", "ParkingLot", "Down", 1);
        hub.Verify<string, string[]>(
            nameof(IMonopolyResponses.PlayerNeedToChooseDirectionEvent),
            (playerId, directions) => playerId == "A" && directions.OrderBy(x => x).SequenceEqual(new[] { "Right", "Down", "Left" }.OrderBy(x => x)));
        hub.VerifyNoElseEvent();
    }

    private void SetupMonopoly(string gameId, Player player, string initialBlockId, Direction initialDirection, int[] dices, string[] landContracts = default!, int remainingSteps = 0)
    {
        var repo = server.GetRequiredService<IRepository>();
        var map = new SevenXSevenMap();
        var game = new Monopoly(gameId, map, Utils.MockDice(dices));

        game.AddPlayer(player, initialBlockId, initialDirection);
        if (landContracts != null)
        {
            foreach (var landContract in landContracts)
            {
                Land? land = map.FindBlockById(landContract) as Land;
                player.AddLandContract(new(player, land));
            }
        }

        game.Initial();
        repo.Save(game);
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
        Player A = new("A", 1000);
        //SetupMonopoly("1", A, "F3", Direction.Up, new[] { 4 });

        var monopolyBuilder = new MonopolyBuilder("1")
        .WithPlayer(
            new MonopolyPlayer(A.Id)
            .WithMoney(A.Money)
            .WithPosition("F3", Direction.Up.ToString())
        )
        .WithMockDice(new[] { 4 })
        .WithCurrentPlayer(nameof(A));

        monopolyBuilder.Save(server);

        var hub = server.CreateHubConnection();

        // Act
        await hub.SendAsync(nameof(MonopolyHub.PlayerRollDice), "1", "A");

        // Assert
        // A 擲了 4 點
        // A 移動到 Station4，方向為 Up，剩下 3 步
        // A 移動到 F4，方向為 Up，剩下 2 步
        // A 移動到 Start，方向為 Right，剩下 1 步
        // A 獲得獎勵金3000，共持有4000元
        // A 移動到 A1，方向為 Right，剩下 0 步
        hub.Verify<string, int>(
            nameof(IMonopolyResponses.PlayerRolledDiceEvent),
            (playerId, diceCount) => playerId == "A" && diceCount == 4);
        Utils.VerifyChessMovedEvent(hub, "A", "Station4", "Up", 3);
        Utils.VerifyChessMovedEvent(hub, "A", "F4", "Up", 2);
        Utils.VerifyChessMovedEvent(hub, "A", "Start", "Right", 1);
        Utils.VerifyChessMovedEvent(hub, "A", "A1", "Right", 0);
        hub.Verify<string, int, decimal>(
            nameof(IMonopolyResponses.ThroughStartEvent),
            (playerId, gainMoney, totalMoney) => playerId == "A" && gainMoney == 3000 && totalMoney == 4000);
        hub.VerifyNoElseEvent();

        // Assert A 有 4000 元
        var repo = server.GetRequiredService<IRepository>();
        var monopoly = repo.FindGameById("1");
        Assert.AreEqual(4000, monopoly.CurrentPlayer!.Money);

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
        Player A = new("A", 1000);
        //SetupMonopoly("1", A, "F3", Direction.Up, new[] { 3 });

        var monopolyBuilder = new MonopolyBuilder("1")
        .WithPlayer(
            new MonopolyPlayer(A.Id)
            .WithMoney(A.Money)
            .WithPosition("F3", Direction.Up.ToString())
        )
        .WithMockDice(new[] { 3 })
        .WithCurrentPlayer(nameof(A));

        monopolyBuilder.Save(server);

        var hub = server.CreateHubConnection();

        // Act
        await hub.SendAsync(nameof(MonopolyHub.PlayerRollDice), "1", "A");


        // Assert
        // A 擲了 3 點
        // A 移動到 Station4，方向為 Up，剩下 2 步
        // A 移動到 F4，方向為 Up，剩下 1 步
        // A 移動到 Start，方向為 Right，剩下 0 步
        // A 沒有獲得獎勵金，共持有1000元 
        hub.Verify<string, int>(
            nameof(IMonopolyResponses.PlayerRolledDiceEvent),
            (playerId, diceCount) => playerId == "A" && diceCount == 3);
        Utils.VerifyChessMovedEvent(hub, "A", "Station4", "Up", 2);
        Utils.VerifyChessMovedEvent(hub, "A", "F4", "Up", 1);
        Utils.VerifyChessMovedEvent(hub, "A", "Start", "Right", 0);
        hub.Verify<string, int, decimal>(
            nameof(IMonopolyResponses.OnStartEvent),
            (playerId, gainMoney, totalMoney) => playerId == "A" && gainMoney == 3000 && totalMoney == 1000);
        hub.VerifyNoElseEvent();

        // A 共持有1000元
        var repo = server.GetRequiredService<IRepository>();
        var monopoly = repo.FindGameById("1");
        Assert.AreEqual(1000, monopoly.CurrentPlayer!.Money);
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
        Player A = new("A");

        var monopolyBuilder = new MonopolyBuilder("1")
        .WithPlayer(
            new MonopolyPlayer(A.Id)
            .WithMoney(A.Money)
            .WithPosition("A1", Direction.Right.ToString())
            .WithLandContract("A2")
        )
        .WithMockDice(new[] { 2 })
        .WithCurrentPlayer(nameof(A));

        monopolyBuilder.Save(server);

        var hub = server.CreateHubConnection();
        // Act
        await hub.SendAsync(nameof(MonopolyHub.PlayerRollDice), "1", "A");
        // Assert
        // A 擲了 2 點
        // A 移動到 A2，方向為 Right，剩下 0 步
        // A 可以蓋房子
        hub.Verify<string, int>(
                       nameof(IMonopolyResponses.PlayerRolledDiceEvent),
                                  (playerId, diceCount) => playerId == "A" && diceCount == 2);
        Utils.VerifyChessMovedEvent(hub, "A", "Station1", "Right", 1);
        Utils.VerifyChessMovedEvent(hub, "A", "A2", "Right", 0);
        hub.Verify<string, string, int, decimal>(
                       nameof(IMonopolyResponses.PlayerCanBuildHouseEvent),
                                  (playerId, blockId, houseCount, upgradeMoney) => playerId == "A" && blockId == "A2" && houseCount == 0 && upgradeMoney == 1000);
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
        Player A = new("A");
        Player B = new("B");

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
        .WithMockDice(new[] { 2 })
        .WithCurrentPlayer("A");

        monopolyBuilder.Save(server);

        var hub = server.CreateHubConnection();
        // Act
        await hub.SendAsync(nameof(MonopolyHub.PlayerRollDice), "1", "A");
        // Assert
        // A 擲了 2 點
        // A 移動到 A2，方向為 Right，剩下 0 步
        // A 需要支付過路費
        hub.Verify<string, int>(
                       nameof(IMonopolyResponses.PlayerRolledDiceEvent),
                                  (playerId, diceCount) => playerId == "A" && diceCount == 2);
        Utils.VerifyChessMovedEvent(hub, "A", "Station1", "Right", 1);
        Utils.VerifyChessMovedEvent(hub, "A", "A2", "Right", 0);
        hub.Verify<string, string, decimal>(
                       nameof(IMonopolyResponses.PlayerPayTollEvent),
                                  (playerId, ownId, toll) => playerId == "A" && ownId == "B" && toll == 50);
        hub.VerifyNoElseEvent();
    }
}