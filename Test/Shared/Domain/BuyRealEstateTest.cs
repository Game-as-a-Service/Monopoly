namespace Shared.Domain;

[TestClass]
public class BuyRealEstateTest
{
    [TestMethod]
    public void test() {
        string a_id = "a";
        string b_id = "b";
        string c_id = "c";

        Player a = new(a_id),
               b = new(b_id),
               c = new(c_id);

        a.AddMoney(1000);
        b.AddMoney(2000);
        c.AddMoney(3000);

        a.AddLandContract(new(1000, a, "A1"));
        var landContract = a.SellLandContract("A1", 500);
        if (!landContract.HasOutcry()) {
            a.AddMoney(landContract.Sell("system", null));
        }

        Assert.AreEqual(a.Money, 1700);
    }
}