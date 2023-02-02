using Shared.Domain;
namespace Shared.Usecases;

[TestClass]
public class GameTest
{
    [TestMethod]
    public void 玩家ABC_玩家BC破產__當遊戲結算__名次為ACB()
    {
        // Arrange
        Game game = new("Test");
        // 玩家 A B C
        var player_a = new Player("A");
        var player_b = new Player("B", 0);
        var player_c = new Player("C", 0);
        game.AddPlayer(player_a);
        game.AddPlayer(player_b);
        game.AddPlayer(player_c);

        // 玩家 B、C 破產
        game.UpdatePlayerState(player_b);
        game.UpdatePlayerState(player_c);
        
        // Act
        // 遊戲結算
        game.Settlement();
        
        // Assert
        // 玩家A獲勝
        Assert.AreEqual(1, game.PlayerRankDictionary[player_a]);
        Assert.AreEqual(3, game.PlayerRankDictionary[player_b]);
        Assert.AreEqual(2, game.PlayerRankDictionary[player_c]);

    }

    [TestMethod]
    public void 玩家ABCD_遊戲時間結束_A的結算金額為5000_B的結算金額為4000_C的結算金額為3000_D的結算金額為2000__當遊戲結算__名次為ABCD()
    {
        // Arrange
        Game game = new("Test");
        // 玩家 A B C D
        var player_a = new Player("A");
        var player_b = new Player("B");
        var player_c = new Player("C");
        var player_d = new Player("D");
        game.AddPlayer(player_a);
        game.AddPlayer(player_b);
        game.AddPlayer(player_c);
        game.AddPlayer(player_d);

        // 玩家 B 的結算金額為 5000
        player_a.AddMoney(5000);

        // 玩家 B 的結算金額為 4000
        player_b.AddMoney(4000);

        // 玩家 C 的結算金額為 3000
        player_c.AddMoney(3000);

        // 玩家 D 的結算金額為 2000
        player_d.AddMoney(2000);

        // Act
        // 遊戲結算
        game.Settlement();
        
        // Assert
        // 名次為 A B C D
        var ranking = game.PlayerRankDictionary;
        Assert.AreEqual(1, ranking[player_a]);
        Assert.AreEqual(2, ranking[player_b]);
        Assert.AreEqual(3, ranking[player_c]);
        Assert.AreEqual(4, ranking[player_d]);
    }
}
