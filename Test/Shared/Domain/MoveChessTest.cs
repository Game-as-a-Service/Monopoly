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
        var game = new Game("Test", map);
        var player = new Player("A");
        game.AddPlayer(player);

        game.SetPlayerToBlock(player, "F4", Map.Direction.Up);
        var point = 6; // TODO: 這邊應該要用骰子的結果

        // Act
        game.PlayerMove(player, point);

        // Assert
        Assert.AreEqual("A4" , game.GetPlayerPosition("A").Id);
    }
}