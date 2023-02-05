using Shared.Domain;

namespace SharedTests.Domain;

[TestClass]
public class MortgageTest
{
    [TestMethod]
    [Description(
        """
        Given:  輪到玩家A
                玩家A持有的金額 5000
                玩家A持有的房地產 A1
                A1價值1000
        When:   玩家A抵押房地產A1
        Then:   玩家A持有金額為5000+1000*70% = 5700
                玩家A持有的房地產 A1
                A1在10回合後收回
        """)]
    public void 玩家抵押房地產()
    {
        // Arrange
        var map = new Map(Utils.SevenXSevenMap());
        Game game = new("g1", map);
        Player a = new("A", 5000);
        game.AddPlayer(a);
        game.Initial();
        Land A1 = (Land)map.FindBlockById("A1");
        a.AddLandContract(new(a, A1));

        // Act
        game.MortgageLandContract("A", "A1");

        // Assert
        Assert.AreEqual(a.Money, 5700);
        Assert.IsNotNull(a.FindLandContract("A1"));
        var mortgage = a.Mortgage.First(m => m.LandContract.Land.Id == "A1");
        Assert.AreEqual(10, mortgage.Deadline);
    }
}
