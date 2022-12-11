namespace Shared.Domain;

[TestClass]
public class BankruptTest
{
    [TestMethod]
    public void 玩家A_A沒錢沒房_已破產() {
        string id_a = "a";
        Player player_a = new(id_a);

        player_a.SetState(PlayerState.Bankrupt);

        Assert.AreEqual(player_a.State, PlayerState.Bankrupt);
    }
}
