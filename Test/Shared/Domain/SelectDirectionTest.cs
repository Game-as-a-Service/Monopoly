namespace Shared.Domain;

[TestClass]
public class SelectDirectionTest
{
    [TestMethod]
    [Description(
        """
        Given:  玩家A目前在停車場
                玩家A方向為Down
        When:   玩家A選擇方向為Left
        Then:   玩家A在停車場
                玩家A方向為Left
        """)]
    public void 玩家選擇方向()
    {
        // Arrange
        var map = new Map(Utils.SevenXSevenMap());
        var game = new Game("Test", map);
        var player = new Player("A");
        game.AddPlayer(player);
        game.Initial();
        game.SetPlayerToBlock(player, "ParkingLot", Map.Direction.Down);

        // Act
        game.PlayerSelectDirection(player, Map.Direction.Left);

        // Assert
        Assert.AreEqual("ParkingLot", game.GetPlayerPosition("A").Id);

        var direction = game.GetPlayerDirection("A");
        Assert.AreEqual(Map.Direction.Left, direction);
    }

    
}