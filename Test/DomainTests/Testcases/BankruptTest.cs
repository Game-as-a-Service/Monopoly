using static Domain.Map;

namespace DomainTests.Testcases;

[TestClass]
public class BankruptTest
{
    [TestMethod]
    public void 玩家A_A沒錢沒房__更新玩家A的狀態__玩家A的狀態為破產()
    {
        string id_a = "a";
        Player player_a = new(id_a, 0);
        var game = new Monopoly("Test");
        game.AddPlayer(player_a, "Start", Direction.Right);
        game.Initial();

        game.UpdatePlayerState(player_a);

        Assert.AreEqual(player_a.State, PlayerState.Bankrupt);
    }
}