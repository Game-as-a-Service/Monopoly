using Domain.Maps;

namespace DomainTests.Testcases;

[TestClass]
public class GameInitialTest
{
    [TestMethod]
    public void 玩家ABCD__遊戲初始設定__每位玩家有15000_每位玩家都會在起點()
    {
        // Arrange
        var map = new SevenXSevenMap();
        Monopoly game = new("Test", map);
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

        Assert.AreEqual("Start", game.GetPlayerPosition("A").Id);
        Assert.AreEqual("Start", game.GetPlayerPosition("B").Id);
        Assert.AreEqual("Start", game.GetPlayerPosition("C").Id);
        Assert.AreEqual("Start", game.GetPlayerPosition("D").Id);
    }
}