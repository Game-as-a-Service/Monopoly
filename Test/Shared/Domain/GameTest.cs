using Shared.Domain;

namespace Shared.Usecases;

[TestClass]
public class GameTest
{
    [TestMethod]
    public void SettlementTest()
    {
        // status code?
        Game game = new();
        // 玩家 A B C
        string id_a = "A";
        string id_b = "B";
        string id_c = "C";
        
        game.AddPlayer(id_a);
        game.AddPlayer(id_b);
        game.AddPlayer(id_c);

        // 玩家 B、C 破產
        // SetState(string, State);
        game.SetState(id_b, PlayerState.Bankrupt);
        game.SetState(id_c, PlayerState.Bankrupt);
        
        // 遊戲結算
        var winner = game.Settlement();
        
        // 玩家A獲勝
        // GetWinner();
        // GetWinner = |self| self.players.filter ...
        // 好吧，這只是我的想法
        // good idea
        var playerA = game.FindPlayerById(id_a);
        Assert.AreEqual(playerA, winner);
    }
}


// Player