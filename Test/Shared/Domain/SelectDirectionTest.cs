namespace Shared.Domain;

[TestClass]
public class SelectDirectionTest
{
    [TestMethod]
    public void 玩家A目前在停車場_方向為Down__選擇方向Left__玩家A在停車場_方向為Left()
    {
        // Arrange
        var map = new Map(Utils.SevenXSevenMap());
        var game = new Game(map);
        var player = new Player("A");
        game.AddPlayer(player);

        game.SetPlayerToBlock(player, "ParkingLot", Map.Direction.Down);

        // Act
        game.PlayerSelectDirection(player, Map.Direction.Left);

        // Assert
        Assert.AreEqual("ParkingLot", game.GetPlayerPosition(player).Id);

        var direction = game.GetPlayerDirection(player);
        Assert.AreEqual(Map.Direction.Left, direction);
    }

    
}