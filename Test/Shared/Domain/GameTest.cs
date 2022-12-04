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
        string id_a = "A";
        string id_b = "B";
        string id_c = "C";
        
        game.AddPlayer(id_a);
        game.AddPlayer(id_b);
        game.AddPlayer(id_c);

        // 玩家 B、C 破產
        game.SetState(id_b, PlayerState.Bankrupt);
        game.SetState(id_c, PlayerState.Bankrupt);
        
        // Act
        // 遊戲結算
        game.Settlement();
        
        // Assert
        // 玩家A獲勝
        var playerA = game.FindPlayerById(id_a);
        Assert.AreEqual(1, game.RankList[playerA]);
        Player playerB = game.FindPlayerById(id_b);
        Assert.AreEqual(3, game.RankList[playerB]);
        Player playerC = game.FindPlayerById(id_c);
        Assert.AreEqual(2, game.RankList[playerC]);

    }

    [TestMethod]
    public void 玩家ABCD_遊戲時間結束_A的結算金額為5000_B的結算金額為4000_C的結算金額為3000_D的結算金額為2000__當遊戲結算__名次為ABCD()
    {
        // Arrange
        Game game = new();
        // 玩家 A B C D
        string id_a = "A";
        string id_b = "B";
        string id_c = "C";
        string id_d = "D";
        
        game.AddPlayer(id_a);
        game.AddPlayer(id_b);
        game.AddPlayer(id_c);
        game.AddPlayer(id_d);

        // 土地+升級+剩餘金額
        // 玩家 A 的結算金額為 5000
        // 升級價格固定為土地購買價
        var playerA = game.FindPlayerById(id_a);
        // 土地價格 持有人(null) 升級次數
        var landContractA1 = new LandContract(2000, playerA);
        landContractA1.Upgrade();
        playerA?.AddLandContract(landContractA1); 
        playerA?.AddMoney(1000);

        // 玩家 B 的結算金額為 4000
        var playerB = game.FindPlayerById(id_b);
        var landContractB1 = new LandContract(2000, playerB);
        landContractB1.Upgrade();
        playerB?.AddLandContract(landContractB1); 

        // 玩家 C 的結算金額為 3000
        var playerC = game.FindPlayerById(id_c);
        var landContractC1 = new LandContract(2000, playerC);
        playerC?.AddLandContract(landContractC1); 
        playerC?.AddMoney(1000);

        // 玩家 D 的結算金額為 2000
        var playerD = game.FindPlayerById(id_d);
        var landContractD1 = new LandContract(2000, playerD);
        playerD?.AddLandContract(landContractD1); 
        
        // Act
        // 遊戲結算
        game.Settlement();
        
        // Assert
        // 名次為 A B C D
        var ranking = game.RankList;
        Assert.AreEqual(1, ranking[playerA]);
        Assert.AreEqual(2, ranking[playerB]);
        Assert.AreEqual(3, ranking[playerC]);
        Assert.AreEqual(4, ranking[playerD]);
    }
}
