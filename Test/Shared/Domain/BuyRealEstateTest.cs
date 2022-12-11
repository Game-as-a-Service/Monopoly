namespace Shared.Domain;

[TestClass]
public class BuyRealEstateTest
{
    [TestMethod]
    public void 玩家ABC_玩家A拍賣A1_賣給系統() {
        string a_id = "a",
               b_id = "b",
               c_id = "c";

        Player a = new(a_id, 1000),
               b = new(b_id, 2000),
               c = new(c_id, 3000);

        a.AddLandContract(new(1000, a, "A1"));
        var landContract = a.SellLandContract("A1");
        if (!landContract.HasOutcry())
            landContract.Sell();

        Assert.AreEqual(a.FindLAndContract("A1"), false);
        Assert.AreEqual(a.Money, 1700);
    }

    [TestMethod]
    public void 玩家ABC_玩家A拍賣A1_B喊價600_B獲得A1() {
        string a_id = "a",
               b_id = "b",
               c_id = "c";

        Player a = new(a_id, 1000),
               b = new(b_id, 2000),
               c = new(c_id, 3000);
        
        a.AddLandContract(new(1000, a, "A1"));
        var landContract = a.SellLandContract("A1");

        landContract.SetOutcry(b, 600);
        landContract.Sell();

        Assert.AreEqual(a.FindLAndContract("A1"), false);
        Assert.AreEqual(a.Money, 1600);
        Assert.AreEqual(b.Money, 1400);
        Assert.AreEqual(b.FindLAndContract("A1"), true);
    }

    [TestMethod]
    public void 玩家ABC_玩家A拍賣A1_B喊價3000因餘額不足不能喊價() {
        string a_id = "a",
               b_id = "b",
               c_id = "c";

        Player a = new(a_id, 1000),
               b = new(b_id, 2000),
               c = new(c_id, 3000);
        
        a.AddLandContract(new(1000, a, "A1"));
        var landContract = a.SellLandContract("A1");

        landContract.SetOutcry(b, 3000);
        landContract.Sell();

        Assert.AreEqual(a.FindLAndContract("A1"), false);
        Assert.AreEqual(a.Money, 1700);
        Assert.AreEqual(b.Money, 2000);
        Assert.AreEqual(b.FindLAndContract("A1"), false);
    }
}