using Shared.Domain;
namespace Shared.Usecases;

[TestClass]
public class GameTest
{
    [TestMethod]
    public void 玩家ABC_玩家BC破產__當遊戲結算__名次為ACB()
    {
        // Arrange
        Game game = new();
        // 玩家 A B C
        var player_a = new Player("A");
        var player_b = new Player("B");
        var player_c = new Player("C");
        game.AddPlayer(player_a);
        game.AddPlayer(player_b);
        game.AddPlayer(player_c);

        // 玩家 B、C 破產
        game.SetState(player_b, PlayerState.Bankrupt);
        game.SetState(player_c, PlayerState.Bankrupt);
        
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
        Game game = new();
        // 玩家 A B C D
        var player_a = new Player("A");
        var player_b = new Player("B");
        var player_c = new Player("C");
        var player_d = new Player("D");
        game.AddPlayer(player_a);
        game.AddPlayer(player_b);
        game.AddPlayer(player_c);
        game.AddPlayer(player_d);

        var landContractA1 = new LandContract(2000, player_a);
        landContractA1.Upgrade();
        player_a.AddLandContract(landContractA1); 
        player_a.AddMoney(1000);

        // 玩家 B 的結算金額為 4000
        var landContractB1 = new LandContract(2000, player_b);
        landContractB1.Upgrade();
        player_b.AddLandContract(landContractB1); 

        // 玩家 C 的結算金額為 3000
        var landContractC1 = new LandContract(2000, player_c);
        player_c.AddLandContract(landContractC1); 
        player_c.AddMoney(1000);

        // 玩家 D 的結算金額為 2000
        var landContractD1 = new LandContract(2000, player_d);
        player_d.AddLandContract(landContractD1); 
        
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
