namespace Shared.Domain;

[TestClass]
public class SelectDirectionTest
{
    [TestMethod]
    public void 玩家A目前在停車場_方向為Down__選擇方向Left__玩家A在停車場_方向為Left()
    {
        // Arrange
        var map = new Map(SevenXSevenMap());
        var game = new Game(map);
        var player = new Player("A");
        game.AddPlayer(player);

        game.SetPlayerToBlock(player, "ParkingLot", Map.Direction.Down);

        // Act
        game.SelectDirection(player, Map.Direction.Left);

        // Assert
        Assert.AreEqual("ParkingLot", game.GetPlayerPosition(player).Id);

        var direction = game.GetPlayerDirection(player);
        Assert.AreEqual(Map.Direction.Left, direction);
    }

    IBlock[][] SevenXSevenMap()
    {
        return new IBlock[][]
        {
            new IBlock[] { new Block("Start"),      new Block("A1"),    new Block("Station1"),  new Block("A2"),    new Block("A3"),            null,               null },
            new IBlock[] { new Block("F4"),         null,               null,                   null,               new Block("A4"),            null,               null },
            new IBlock[] { new Block("Station4"),   null,               new Block("B5"),        new Block("B6"),    new Block("ParkingLot"),    new Block("C1"),    new Block("C2") },
            new IBlock[] { new Block("F3"),         null,               new Block("B4"),        null,               new Block("B1"),            null,               new Block("C3") },
            new IBlock[] { new Block("F2"),         new Block("F1"),    new Block("Prison"),    new Block("B3"),    new Block("B2"),            null,               new Block("Station2") },
            new IBlock[] { null,                    null,               new Block("E3"),        null,               null,                       null,               new Block("D1") },
            new IBlock[] { null,                    null,               new Block("E2"),        new Block("E1"),    new Block("Station3"),      new Block("D3"),    new Block("D2") },
        };
    }
}