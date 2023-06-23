using Domain.Events;
using Domain.Maps;

namespace DomainTests.Testcases;

[TestClass]
public class SelectDirectionTest
{
    [TestMethod]
    [Description(
        """
        Given:  玩家A目前在停車場
                玩家A方向為Down
                玩家A需要選擇方向
        When:   玩家A選擇方向為Left
        Then:   玩家A在停車場
                玩家A方向為Left
        """)]
    public void 玩家選擇方向()
    {
        // Arrange
        var map = new SevenXSevenMap();
        var game = new Monopoly("Test", map);
        var player = new Player("A");
        game.AddPlayer(player, "ParkingLot", Map.Direction.Down);
        game.Initial();

        // Act
        game.PlayerSelectDirection("A", "Left");

        // Assert
        Assert.AreEqual("ParkingLot", game.GetPlayerPosition("A").Id);

        var direction = game.GetPlayerDirection("A");
        Assert.AreEqual(Map.Direction.Left, direction);
    }

    [TestMethod]
    [Description(
        """
        Given:  玩家A目前在停車場
                玩家A方向為Down
                玩家A目前還能走3步
                玩家A需要選擇方向
        When:   玩家A選擇方向為Left
        Then:   玩家A在B4
                玩家A方向為Down
        """)]
    public void 玩家選擇方向後會繼續前進到定點()
    {
        // Arrange
        var map = new SevenXSevenMap();
        var game = new Monopoly("Test", map);
        var player = new Player("A");
        game.AddPlayer(player, "ParkingLot", Map.Direction.Down);
        game.Initial();
        var chess = game.CurrentPlayer!.Chess;
        game.CurrentPlayer.Chess = new Chess(player, map, chess.CurrentBlock, chess.CurrentDirection, 3);

        // Act
        game.PlayerSelectDirection("A", "Left");

        // Assert
        Assert.AreEqual("B4", game.GetPlayerPosition("A").Id);

        var direction = game.GetPlayerDirection("A");
        Assert.AreEqual(Map.Direction.Down, direction);
    }

    [TestMethod]
    [Description(
        """
        Given:  玩家A目前在停車場
                玩家A方向為Down
                玩家A目前還能走4步
                玩家A需要選擇方向
        When:   玩家A選擇方向為Left
        Then:   玩家A在監獄
                玩家A需要選擇方向
        """)]
    public void 玩家選擇方向後會繼續前進到需要選擇方向的地方()
    {
        // Arrange
        var map = new SevenXSevenMap();
        var game = new Monopoly("Test", map);
        var player = new Player("A");
        game.AddPlayer(player, "ParkingLot", Map.Direction.Down);
        game.Initial();
        var chess = game.CurrentPlayer!.Chess;
        game.CurrentPlayer.Chess = new Chess(player, map, chess.CurrentBlock, chess.CurrentDirection, 4);

        // Act
        game.PlayerSelectDirection("A", "Left");

        // Assert
        Assert.AreEqual("Jail", game.GetPlayerPosition("A").Id);
        Assert.IsTrue(game.DomainEvents.Any(
            e => e is PlayerNeedToChooseDirectionEvent));
    }

    [TestMethod]
    [Description(
        """
        Given:  玩家A目前在監獄
                玩家A方向為Down
                玩家A目前還能走0步
                玩家A需要選擇方向
        When:   玩家A選擇方向為Left
        Then:   玩家A在監獄
                玩家A沒有需要選擇方向的事件
        """)]
    public void 玩家選擇方向後會停止()
    {
        // Arrange
        var map = new SevenXSevenMap();
        var game = new Monopoly("Test", map);
        var player = new Player("A");
        game.AddPlayer(player, "Jail", Map.Direction.Down);
        game.Initial();
        var chess = game.CurrentPlayer!.Chess;
        game.CurrentPlayer.Chess = new Chess(player, map, chess.CurrentBlock, chess.CurrentDirection, 0);

        // Act
        game.PlayerSelectDirection("A", "Left");

        // Assert
        Assert.AreEqual("Jail", game.GetPlayerPosition("A").Id);
        Assert.IsFalse(game.DomainEvents.Any(
            e => e is PlayerNeedToChooseDirectionEvent));
    }
}