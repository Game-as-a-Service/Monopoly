﻿using Application.Common;
using Domain;
using Domain.Maps;
using Server.Hubs;
using static Domain.Map;

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
        SetupMonopoly("1", A, "F4", Direction.Up, new[] { 6 });

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
        Player A = new("A");
        SetupMonopoly("1", A, "F4", Direction.Up, new[] { 2, 6 });

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
        VerifyChessMovedEvent(hub, "A", "Start", "Right", 7);
        VerifyChessMovedEvent(hub, "A", "A1", "Right", 6);
        hub.Verify<string, int, decimal>(
            nameof(IMonopolyResponses.ThroughStartEvent),
            (playerId, gainMoney, totalMoney) => playerId == "A" && gainMoney == 3000 && totalMoney == 18000);
        VerifyChessMovedEvent(hub, "A", "Station1", "Right", 5);
        VerifyChessMovedEvent(hub, "A", "A2", "Right", 4);
        VerifyChessMovedEvent(hub, "A", "A3", "Down", 3);
        VerifyChessMovedEvent(hub, "A", "A4", "Down", 2);
        VerifyChessMovedEvent(hub, "A", "ParkingLot", "Down", 1);
        hub.Verify<string, string[]>(
            nameof(IMonopolyResponses.PlayerNeedToChooseDirectionEvent),
            (playerId, directions) => playerId == "A" && directions.OrderBy(x => x).SequenceEqual(new[] { "Right", "Down", "Left" }.OrderBy(x => x)));
        hub.VerifyNoElseEvent();
    }

    private void SetupMonopoly(string gameId, Player player, string initialBlockId, Direction initialDirection, int[] dices)
    {
        var repo = server.GetRequiredService<IRepository>();
        var game = new Monopoly(gameId, new SevenXSevenMap(), Utils.MockDice(dices));

        game.AddPlayer(player, initialBlockId, initialDirection);

        game.Initial();
        repo.Save(game);
    }

    static void VerifyChessMovedEvent(VerificationHub hub, string playerId, string blockId, string direction, int remainingSteps)
    {
        hub.Verify<string, string, string, int>(nameof(IMonopolyResponses.ChessMovedEvent), (PlayerId, BlockId, Direction, RemainingSteps) =>
            PlayerId == playerId && BlockId == blockId && Direction == direction && RemainingSteps == remainingSteps);
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
        SetupMonopoly("1", A, "F3", Direction.Up, new[] { 4 });

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
        VerifyChessMovedEvent(hub, "A", "Station4", "Up", 3);
        VerifyChessMovedEvent(hub, "A", "F4", "Up", 2);
        VerifyChessMovedEvent(hub, "A", "Start", "Right", 1);
        VerifyChessMovedEvent(hub, "A", "A1", "Right", 0);
        hub.Verify<string, int, decimal>(
            nameof(IMonopolyResponses.ThroughStartEvent),
            (playerId, gainMoney, totalMoney) => playerId == "A" && gainMoney == 3000 && totalMoney == 4000);
        hub.VerifyNoElseEvent();

        // Assert A 有 4000 元
        var repo = server.GetRequiredService<IRepository>();
        var monopoly = repo.FindGameById("1");
        Assert.AreEqual(4000, monopoly.CurrentPlayer!.Money);

    }
}