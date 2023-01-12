using static Shared.Domain.Map;

namespace Shared.Domain;

[TestClass]
public class MoveChessTest
{
    [TestMethod]
    public void 玩家擲骰得到6點_目前在F4__系統移動棋子__A_移動到_A4()
    {
        // Arrange
        var map = new Map(Utils.SevenXSevenMap());
        // 灌了水銀的骰子 1 顆, 只會骰 6 點
        var game = new Game("Test", map, new DiceSetting(1, 6, 6));
        var player = new Player("A");
        game.AddPlayer(player);
        game.Initial();
        game.SetPlayerToBlock(player, "F4", Direction.Up);

        // Act
        game.PlayerRollDice(player.Id);
        game.PlayerMoveChess(player.Id);

        // Assert
        Assert.AreEqual("A4" , game.GetPlayerPosition("A").Id);
    }
}