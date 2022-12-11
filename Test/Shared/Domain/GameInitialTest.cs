namespace Shared.Domain;

[TestClass]
public class GameInitialTest
{
    [TestMethod]
    public void 玩家ABCD__遊戲初始設定__每位玩家有15000_每位玩家都會在起點()
    {
        // Arrange
        var map = new Map(SevenXSevenMap());
        Game game = new(map);
        // 玩家 A B C D
        var player_a = new Player("A");
        var player_b = new Player("B");
        var player_c = new Player("C");
        var player_d = new Player("D");
        game.AddPlayer(player_a);
        game.AddPlayer(player_b);
        game.AddPlayer(player_c);
        game.AddPlayer(player_d);

        // Act
        game.Initial();

        // Assert
        Assert.AreEqual(15000, player_a.Money);
        Assert.AreEqual(15000, player_b.Money);
        Assert.AreEqual(15000, player_c.Money);
        Assert.AreEqual(15000, player_d.Money);

        Assert.AreEqual("Start", game.GetPlayerPosition(player_a).Id);
        Assert.AreEqual("Start", game.GetPlayerPosition(player_a).Id);
        Assert.AreEqual("Start", game.GetPlayerPosition(player_a).Id);
        Assert.AreEqual("Start", game.GetPlayerPosition(player_a).Id);

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