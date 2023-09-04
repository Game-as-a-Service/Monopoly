using Domain.Builders;
using Domain.Events;

namespace DomainTests.Testcases;

[TestClass]
public class RollDiceTest
{
    private static Map Map => new Domain.Maps.SevenXSevenMap();
    [TestMethod]
    [Description(
        """
        Given:  目前 A 在 F4，方向 Up
                A 持有 2000 元
        When:   A 擲骰得到 6 點
        Then:   A 移動到 A4，方向 Down
                A 剩餘步數為 0

                DomainEvent:    1. A 擲骰子得到了 6 點
                                2. A 移動到 Start，方向 Up，剩餘 5 步
                                3. A 獲得 3000 元，還有 5000 元
                                4. A 移動到 A1，方向 Right，剩餘 4 步
                                5. A 移動到 Station1，方向 Right，剩餘 3 步
                                6. A 移動到 A2，方向 Right，剩餘 2 步
                                7. A 移動到 A3，方向 Right，剩餘 1 步
                                8. A 移動到 A4，方向 Down，剩餘 0 步
                                9. A 可以買下 A4，價錢 1000
        
        """)]
    public void 玩家擲骰後移動棋子()
    {
        // Arrange
        var A = new { Id = "A", CurrentBlockId = "F4", Direction = "Up", Money = 2000m };
        var dicePoints = 6;
        var expected = new
        {
            BlockId = "A4",
            Direction = "Down",
            RemainingSteps = 0
        };

        var player = new PlayerBuilder(A.Id)
            .WithMoney(A.Money)
            .WithPosition(A.CurrentBlockId, A.Direction)
            .Build();
        var monopoly = new MonopolyBuilder()
            .WithMap(Map)
            .WithPlayer(player)
            .WithDices(Utils.MockDice(dicePoints))
            .WithCurrentPlayer(new CurrentPlayerState(A.Id))
            .Build();

        // Act
        monopoly.PlayerRollDice(player.Id);

        // Assert
        確認玩家目前位置(monopoly, player.Id, expected.BlockId, expected.Direction);
        確認玩家剩餘步數(monopoly, player.Id, expected.RemainingSteps);
        monopoly.DomainEvents
            .NextShouldBe(new PlayerRolledDiceEvent(player.Id, dicePoints))
            .NextShouldBe(new ChessMovedEvent(player.Id, "Start", "Up", 5))
            .NextShouldBe(new ThroughStartEvent(player.Id, 3000, 5000m))
            .NextShouldBe(new ChessMovedEvent(player.Id, "A1", "Right", 4))
            .NextShouldBe(new ChessMovedEvent(player.Id, "Station1", "Right", 3))
            .NextShouldBe(new ChessMovedEvent(player.Id, "A2", "Right", 2))
            .NextShouldBe(new ChessMovedEvent(player.Id, "A3", "Right", 1))
            .NextShouldBe(new ChessMovedEvent(player.Id, "A4", "Down", 0))
            .NextShouldBe(new PlayerCanBuyLandEvent(player.Id, "A4", 1000m))
            .NoMore();
    }

    [TestMethod]
    [Description(
        """
        Given:  目前 A 在 F4
        When:   玩家擲骰得到 8 點
        Then:   A 移動到 停車場，方向 Down
                A 剩餘步數為 1
                A 需要選擇方向
        """)]
    public void 玩家擲骰後移動棋子到需要選擇方向的地方()
    {
        // Arrange
        var A = new { Id = "A", CurrentBlockId = "F4", Direction = "Up" };
        var dicePoints = 8;
        var expected = new
        {
            BlockId = "ParkingLot",
            Direction = "Down",
            RemainingSteps = 1
        };

        var player = new PlayerBuilder(A.Id)
            .WithPosition(A.CurrentBlockId, A.Direction)
            .Build();
        var monopoly = new MonopolyBuilder()
            .WithMap(Map)
            .WithPlayer(player)
            .WithCurrentPlayer(new CurrentPlayerState(A.Id))
            .WithDices(Utils.MockDice(dicePoints))
            .Build();

        // Act
        monopoly.PlayerRollDice(player.Id);

        //Assert
        確認玩家目前位置(monopoly, player.Id, expected.BlockId, expected.Direction);
        確認玩家剩餘步數(monopoly, player.Id, expected.RemainingSteps);
        Assert.IsTrue(monopoly.DomainEvents.Any(
            e => e is PlayerNeedToChooseDirectionEvent @event &&
                            @event.PlayerId == player.Id &&
                            @event.Directions.Count() == 3
                            ));
    }

    [TestMethod]
    [Description(
        """
        Given:  目前 A 在 F3
                A 持有 1000 元
        When:   A 擲骰得到 4 點
        Then:   A 移動到 A1
                A 剩餘步數為 0
                A 持有 4000 元
        """)]
    public void 玩家擲骰後移動棋子經過起點獲得獎勵金3000()
    {
        // Arrange
        var A = new { Id = "A", CurrentBlockId = "F3", Direction = "Up" };
        var dicePoints = 4;
        var expected = new
        {
            BlockId = "A1",
            Direction = "Up",
            RemainingSteps = 0,
            Money = 4000
        };

        var player = new PlayerBuilder(A.Id)
            .WithPosition(A.CurrentBlockId, A.Direction)
            .WithMoney(1000)
            .Build();
        var monopoly = new MonopolyBuilder()
            .WithMap(Map)
            .WithPlayer(player)
            .WithCurrentPlayer(new CurrentPlayerState(A.Id))
            .WithDices(Utils.MockDice(dicePoints))
            .Build();

        // Act
        monopoly.PlayerRollDice(player.Id);

        // Assert
        確認玩家目前位置(monopoly, player.Id, expected.BlockId, expected.Direction);
        確認玩家剩餘步數(monopoly, player.Id, expected.RemainingSteps);
        確認玩家持有金額(monopoly, player.Id, expected.Money);
    }

    [TestMethod]
    [Description(
        """
        Given:  目前 A 在 F3，方向 Up
                A 持有 1000 元
        When:   A 擲骰得到 3 點
        Then:   A 移動到 起點，方向 Up
                A 剩餘步數為 0
                A 持有 1000 元
        """)]
    public void 玩家擲骰後移動棋子到起點無法獲得獎勵金()
    {
        // Arrange
        var A = new { Id = "A", CurrentBlockId = "F3", Direction = "Up" };
        var dicePoints = 3;
        var expected = new
        {
            BlockId = "Start",
            Direction = "Up",
            RemainingSteps = 0,
            Money = 1000
        };

        var player = new PlayerBuilder(A.Id)
            .WithPosition(A.CurrentBlockId, A.Direction)
            .WithMoney(1000)
            .Build();
        var monopoly = new MonopolyBuilder()
            .WithMap(Map)
            .WithPlayer(player)
            .WithCurrentPlayer(new CurrentPlayerState(A.Id))
            .WithDices(Utils.MockDice(dicePoints))
            .Build();

        // Act
        monopoly.PlayerRollDice(player.Id);

        // Assert
        確認玩家目前位置(monopoly, player.Id, expected.BlockId, expected.Direction);
        確認玩家剩餘步數(monopoly, player.Id, expected.RemainingSteps);
        確認玩家持有金額(monopoly, player.Id, expected.Money);
    }

    [TestMethod]
    [Description("""
        Given:  目前 A 在 A1，方向 Right
                A 持有 A2
                A2 房子有 3 間
        When:   A 擲骰得到 2 點
        Then:   A 移動到 A2，方向 Right
                A 剩餘步數為 0
                提示可以蓋房子
        """)]
    public void 玩家擲骰後移動棋子到自己擁有地()
    {
        // Arrange
        var A = new { Id = "A", CurrentBlockId = "A1", Direction = "Right"};
        var A2 = new { Id = "A2", HouseCount = 3 };
        var dicePoints = 2;
        var expected = new
        {
            BlockId = "A2",
            Direction = "Right",
            RemainingSteps = 0
        };

        var player = new PlayerBuilder(A.Id)
            .WithPosition(A.CurrentBlockId, A.Direction)
            .WithLandContract(A2.Id)
            .Build();
        var monopoly = new MonopolyBuilder()
            .WithMap(Map)
            .WithPlayer(player)
            .WithCurrentPlayer(new CurrentPlayerState(player.Id))
            .WithDices(Utils.MockDice(dicePoints))
            .Build();

        // Act
        monopoly.PlayerRollDice(player.Id);

        // Assert
        確認玩家目前位置(monopoly, player.Id, expected.BlockId, expected.Direction);
        確認玩家剩餘步數(monopoly, player.Id, expected.RemainingSteps);
        
        Assert.IsTrue(monopoly.DomainEvents.Any(
                       e => e is PlayerCanBuildHouseEvent @event &&
                               @event.PlayerId == player.Id &&
                               @event.BlockId == "A2" &&
                               @event.HouseCount == 0 &&
                               @event.UpgradeMoney == 1000));
    }

    
    private static void 確認玩家目前位置(Monopoly monopoly, string playerId, string blockId, string direction)
    {
        Assert.AreEqual(blockId, monopoly.Players.First(p => p.Id == playerId).Chess.CurrentBlockId);
        Assert.AreEqual(direction, monopoly.Players.First(p => p.Id == playerId).Chess.CurrentDirection.ToString());
    }

    private static void 確認玩家剩餘步數(Monopoly monopoly, string playerId, int expectedRemainingSteps)
    {
        Assert.AreEqual(expectedRemainingSteps, monopoly.Players.First(p => p.Id == playerId).Chess.RemainingSteps);
    }
    private static void 確認玩家持有金額(Monopoly monopoly, string id, int expectedMoney)
    {
        Assert.AreEqual(expectedMoney, monopoly.Players.First(p => p.Id == id).Money);
    }
}