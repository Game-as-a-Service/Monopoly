using Domain.Builders;
using Domain.Events;
using Domain.Maps;

namespace DomainTests.Testcases;

[TestClass]
public class SelectDirectionTest
{
    private static Map Map => new Domain.Maps.SevenXSevenMap();

    [TestMethod]
    [Description(
        """
        Given:  玩家 A 目前在停車場 方向 Down 剩餘 0 步
                玩家 A 需要選擇方向
        When:   玩家 A 選擇方向為 Left
        Then:   玩家 A 在停車場
                玩家 A 方向為 Left
                玩家 A 剩餘 0 步

                DomainEvent:    A 選擇方向為 Left
        """)]
    public void 玩家選擇方向()
    {
        // Arrange
        var A = new { Id = "A", CurrentBlockId = "ParkingLot", CurrentDirection = "Down", RemainingSteps = 0 };
        var direction = "Left";
        var expected = new { Id = "A", CurrentBlockId = "ParkingLot", CurrentDirection = "Left", RemainingSteps = 0 };

        var monopoly = new MonopolyBuilder()
            .WithMap(Map)
            .WithPlayer(A.Id, p => p.WithPosition(A.CurrentBlockId, A.CurrentDirection)
                                    .WithRemainingSteps(A.RemainingSteps))
            .WithCurrentPlayer(A.Id, x => x.WithSelectedDirection(false))
            .Build();

        // Act
        monopoly.PlayerSelectDirection(A.Id, direction);

        // Assert
        確認玩家目前位置(monopoly, A.Id, expected.CurrentBlockId, expected.CurrentDirection);
        確認玩家剩餘步數(monopoly, A.Id, expected.RemainingSteps);

        monopoly.DomainEvents
            .NextShouldBe(new PlayerChooseDirectionEvent(A.Id, direction))
            .NoMore();
    }

    [TestMethod]
    [Description(
        """
        Given:  玩家 A 目前在 停車場
                玩家 A 方向為 Down
                玩家 A 目前還能走 3 步
                玩家 A 需要選擇方向
        When:   玩家 A 選擇方向為 Left
        Then:   玩家 A 在 B4
                玩家 A 方向為 Down
                玩家 A 剩餘 0 步

                DomainEvent:    A 選擇方向為 Left
                                A 移動到 B6，方向 Left，剩餘 2 步
                                A 移動到 B5，方向 Left，剩餘 1 步
                                A 移動到 B4，方向 Down，剩餘 0 步
                                A 可以買下 B4，價格 1000 元
        """)]
    public void 玩家選擇方向後會繼續前進到定點()
    {
        // Arrange
        var A = new { Id = "A", CurrentBlockId = "ParkingLot", CurrentDirection = "Down", RemainingSteps = 3 };
        var direction = "Left";
        var expected = new { Id = "A", CurrentBlockId = "B4", CurrentDirection = "Down", RemainingSteps = 0 };

        var monopoly = new MonopolyBuilder()
            .WithMap(Map)
            .WithPlayer(A.Id, p => p.WithPosition(A.CurrentBlockId, A.CurrentDirection)
                                    .WithRemainingSteps(A.RemainingSteps))
            .WithCurrentPlayer(A.Id, x => x.WithSelectedDirection(false))
            .Build();

        // Act
        monopoly.PlayerSelectDirection(A.Id, direction);

        // Assert
        確認玩家目前位置(monopoly, A.Id, expected.CurrentBlockId, expected.CurrentDirection);
        確認玩家剩餘步數(monopoly, A.Id, expected.RemainingSteps);

        monopoly.DomainEvents
            .NextShouldBe(new PlayerChooseDirectionEvent(A.Id, direction))
            .NextShouldBe(new ChessMovedEvent(A.Id, "B6", "Left", 2))
            .NextShouldBe(new ChessMovedEvent(A.Id, "B5", "Down", 1))
            .NextShouldBe(new ChessMovedEvent(A.Id, "B4", "Down", 0))
            .NextShouldBe(new PlayerCanBuyLandEvent(A.Id, "B4", 1000))
            .NoMore();
    }

    [TestMethod]
    [Description(
        """
        Given:  玩家 A 目前在 停車場
                玩家 A 方向為 Down
                玩家 A 目前還能走 4 步
                玩家 A 需要選擇方向
        When:   玩家 A 選擇方向為 Left
        Then:   玩家 A 在監獄，方向 Down
                玩家 A 需要選擇方向

                DomainEvent:    A 選擇方向為 Left
                                A 移動到 B6，方向 Left，剩餘 3 步
                                A 移動到 B5，方向 Left，剩餘 2 步
                                A 移動到 B4，方向 Down，剩餘 1 步
                                A 移動到 Jail，方向 Down，剩餘 0 步
                                A 需要選擇方向 Left、Right、Down
        """)]
    public void 玩家選擇方向後會繼續前進到需要選擇方向的地方()
    {
        // Arrange
        var A = new { Id = "A", CurrentBlockId = "ParkingLot", CurrentDirection = "Down", RemainingSteps = 4 };
        var direction = "Left";
        var expected = new { Id = "A", CurrentBlockId = "Jail", CurrentDirection = "Down", RemainingSteps = 0 };
        
        var monopoly = new MonopolyBuilder()
            .WithMap(Map)
            .WithPlayer(A.Id, p => p.WithPosition(A.CurrentBlockId, A.CurrentDirection)
                                    .WithRemainingSteps(A.RemainingSteps))
            .WithCurrentPlayer(A.Id, x => x.WithSelectedDirection(false))
            .Build();

        // Act
        monopoly.PlayerSelectDirection("A", "Left");

        // Assert
        確認玩家目前位置(monopoly, A.Id, expected.CurrentBlockId, expected.CurrentDirection);
        確認玩家剩餘步數(monopoly, A.Id, expected.RemainingSteps);

        monopoly.DomainEvents
            .NextShouldBe(new PlayerChooseDirectionEvent(A.Id, direction))
            .NextShouldBe(new ChessMovedEvent(A.Id, "B6", "Left", 3))
            .NextShouldBe(new ChessMovedEvent(A.Id, "B5", "Down", 2))
            .NextShouldBe(new ChessMovedEvent(A.Id, "B4", "Down", 1))
            //.NextShouldBe(new ChessMovedEvent(A.Id, "Jail", "Down", 0))
            .NextShouldBe(new PlayerNeedToChooseDirectionEvent(A.Id, "Left", "Right", "Down"))
            .NoMore();
    }

    [TestMethod]
    [Description(
        """
        Given:  玩家 A 目前在 監獄
                玩家 A 方向為 Down
                玩家 A 目前還能走 0 步
                玩家 A 需要選擇方向
        When:   玩家 A 選擇方向為 Left
        Then:   玩家 A 在監獄，方向 Left
                玩家 A 暫停 2 回合

                DomainEvent:    A 選擇方向為 Left
                                A 因踩到監獄，暫停 2 回合
        """)]
    public void 玩家選擇方向後會停止()
    {
        // Arrange
        var A = new { Id = "A", CurrentBlockId = "Jail", CurrentDirection = "Down", RemainingSteps = 0 };
        var direction = "Left";
        var expected = new { Id = "A", CurrentBlockId = "Jail", CurrentDirection = "Left", RemainingSteps = 0 };

        var monopoly = new MonopolyBuilder()
            .WithMap(Map)
            .WithPlayer(A.Id, p => p.WithPosition(A.CurrentBlockId, A.CurrentDirection)
                                    .WithRemainingSteps(A.RemainingSteps))
            .WithCurrentPlayer(A.Id, x => x.WithSelectedDirection(false))
            .Build();

        // Act
        monopoly.PlayerSelectDirection("A", "Left");

        // Assert
        確認玩家目前位置(monopoly, A.Id, expected.CurrentBlockId, expected.CurrentDirection);
        確認玩家剩餘步數(monopoly, A.Id, expected.RemainingSteps);

        monopoly.DomainEvents
            .NextShouldBe(new PlayerChooseDirectionEvent(A.Id, direction))
            .NextShouldBe(new SuspendRoundEvent(A.Id, 2))
            .NoMore();
    }

    [TestMethod]
    [Description(
        """
        Given   玩家 A 目前在 停車場
                玩家 A 方向為 Left
                玩家 A 已選擇方向 Left
        When    玩家 A 選擇方向為 Right
        Then    玩家無法再次選擇方向

                DomainEvent:    A 已經選擇過方向了
        """)]
    public void 玩家選擇方向後不能再次選擇方向()
    {
        // Arrange
        var A = new { Id = "A", CurrentBlockId = "ParkingLot", CurrentDirection = "Left", RemainingSteps = 0 };
        var direction = "Right";
        var expected = new { Id = "A", CurrentBlockId = "ParkingLot", CurrentDirection = "Left", RemainingSteps = 0 };

        var monopoly = new MonopolyBuilder()
            .WithMap(Map)
            .WithPlayer(A.Id, p => p.WithPosition(A.CurrentBlockId, A.CurrentDirection)
                                    .WithRemainingSteps(A.RemainingSteps))
            .WithCurrentPlayer(A.Id)
            .Build();

        // Act
        monopoly.PlayerSelectDirection(A.Id, direction);

        // Assert
        確認玩家目前位置(monopoly, A.Id, expected.CurrentBlockId, expected.CurrentDirection);
        確認玩家剩餘步數(monopoly, A.Id, expected.RemainingSteps);

        monopoly.DomainEvents
            .NextShouldBe(new PlayerHadSelectedDirectionEvent(A.Id))
            .NoMore();
    }

    [TestMethod]
    [Description(
               """
        Given   玩家 A 目前在 停車場
                玩家 A 方向為 Down
                玩家 A 需要選擇方向
        When    玩家 A 選擇方向為 Up
        Then    玩家無法選擇反方向走回頭路

                DomainEvent:    A 無法選擇此方向
        """)]
    public void 玩家無法選擇反方向走回頭路()
    {
        // Arrange
        var A = new { Id = "A", CurrentBlockId = "ParkingLot", CurrentDirection = "Down", RemainingSteps = 0 };
        var direction = "Up";
        var expected = new { Id = "A", CurrentBlockId = "ParkingLot", CurrentDirection = "Down", RemainingSteps = 0 };

        var monopoly = new MonopolyBuilder()
            .WithMap(Map)
            .WithPlayer(A.Id, p => p.WithPosition(A.CurrentBlockId, A.CurrentDirection)
                                    .WithRemainingSteps(A.RemainingSteps))
            .WithCurrentPlayer(A.Id, x => x.WithSelectedDirection(false))
            .Build();

        // Act
        monopoly.PlayerSelectDirection(A.Id, direction);

        // Assert
        確認玩家目前位置(monopoly, A.Id, expected.CurrentBlockId, expected.CurrentDirection);
        確認玩家剩餘步數(monopoly, A.Id, expected.RemainingSteps);

        monopoly.DomainEvents
            .NextShouldBe(new PlayerChooseInvalidDirectionEvent(A.Id, direction))
            .NoMore();
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
    private static void 確認玩家持有金額(Monopoly monopoly, string id, decimal expectedMoney)
    {
        Assert.AreEqual(expectedMoney, monopoly.Players.First(p => p.Id == id).Money);
    }
}