namespace Shared.Domain;

[TestClass]
public class MoveChessTest
{
    [TestMethod]
    public void 玩家擲骰得到6點_目前在F4__系統移動棋子__A_移動到_A4()
    {
        // Arrange
        var map = new Map(SevenXSevenMap());
        var game = new Game(map);
        var player = new Player("A");
        game.AddPlayer(player);

        game.Map.PlayerMove(player, "F4", Map.Direction.Up);
        var point = 6; // TODO: 這邊應該要用骰子的結果

        // Act
        game.Map.PlayerMove(player, point);

        // Assert
        Assert.AreEqual("A4", game.Map.GetPlayerPositionBlock(player).Id);
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