using Domain.Maps;
using static Domain.Map;

namespace DomainTests.Testcases;

[TestClass]
public class BuyLandTest
{
    [TestMethod]
    [Description(
        """
        Given:  玩家A資產5000元，沒有房地產
                目前輪到A，A在F4上
                A1是空地，購買價1000元
        When:   玩家A購買土地
        Then:   玩家A持有金額為4000
                玩家A持有房地產數量為1
                玩家A持有房地產為F4
        """)]
    public void 玩家在空地上可以購買土地()
    {
        // Arrange
        // 玩家A持有的金額 5000
        // 玩家A目前踩在價值為1000的土地F4上
        var map = new SevenXSevenMap();
        var game = new Monopoly("Test", map);
        var playerA = new Player("A", 5000);
        game.AddPlayer(playerA, "F4", Direction.Up);
        game.Initial();

        // Act
        // 玩家A進行購買F4
        game.BuyLand(playerA, "F4");

        // Assert
        // 玩家A持有金額為4000
        // 玩家A持有的房地產 F4
        Assert.AreEqual(4000, playerA.Money);
        Assert.AreEqual(1, playerA.LandContractList.Count());
        Assert.IsTrue(playerA.LandContractList.Any(pa => pa.Land.Id == "F4"));
    }

    [TestMethod]
    [Description(
        """
        Given:  玩家A資產500元,沒有房地產
                目前輪到A,A在F4上
                A1是空地,購買價1000元
        When:   玩家A購買土地
        Then:   顯示錯誤訊息"金額不足"
                玩家A持有金額為500
                玩家A持有房地產數量為0
        """)]
    public void 金錢不夠無法購買土地()
    {
        // Arrange
        // 玩家A持有的金額 500
        // 玩家A目前踩在價值為1000的土地F4上
        var map = new SevenXSevenMap();
        var game = new Monopoly("Test", map);
        var playerA = new Player("A", 500);
        game.AddPlayer(playerA);
        game.Initial();

        // Act
        //顯示錯誤訊息"金額不足"
        Assert.ThrowsException<Exception>(() => game.BuyLand(playerA, "F4"), "金額不足");

        // Assert
        // 玩家A持有金額為500
        // 玩家A無持有的房地產
        Assert.AreEqual(500, playerA.Money);
        Assert.AreEqual(0, playerA.LandContractList.Count());
    }

    [TestMethod]
    [Description(
        """
        Given:  玩家A資產5000元，沒有房地產
                玩家B資產5000元,擁有房地產F4
                目前輪到A，A在F4上
        When:   玩家A購買土地F4
        Then:   顯示錯誤訊息"非空地"
                玩家A持有金額為5000,持有房地產數量為0
                玩家B持有金額為5000,持有房地產數量為1,持有F4
        """)]
    public void 無法購買非空地土地()
    {
        // Arrange
        // 玩家A持有的金額 5000
        // 玩家A目前踩在價值為1000的土地F4上
        var map = new SevenXSevenMap();
        var game = new Monopoly("Test", map);
        var playerA = new Player("A", 5000);
        var playerB = new Player("B", 5000);
        game.AddPlayer(playerA);
        game.AddPlayer(playerB);
        game.Initial();
        Land F4 = (Land)map.FindBlockById("F4");
        playerB.AddLandContract(new(playerB, F4));

        // Act
        // 玩家B進行購買F4
        Assert.ThrowsException<Exception>(() => game.BuyLand(playerA, "F4"), "非空地");

        // Assert
        // 玩家A持有金額為5000
        // 玩家A無持有的房地產
        Assert.AreEqual(5000, playerA.Money);
        Assert.AreEqual(0, playerA.LandContractList.Count());
        // 玩家B持有金額為5000
        // 玩家B持有的房地產F4
        Assert.AreEqual(5000, playerB.Money);
        Assert.AreEqual(1, playerB.LandContractList.Count());
        Assert.IsTrue(playerB.LandContractList.Any(pa => pa.Land.Id == "F4"));
    }


    [TestMethod]
    [Description(
        """
        Given:  玩家A資產5000元，沒有房地產
                目前輪到A，A在F2上
        When:   玩家A購買土地F4
        Then:   顯示錯誤訊息"必須在購買的土地上才可以購買"
                玩家A持有金額為5000,持有房地產數量為0
        """)]
    public void 無法購買非腳下的土地()
    {
        // Arrange
        // 玩家A持有的金額 5000
        // 玩家A目前踩在價值為1000的土地F4上
        var map = new SevenXSevenMap();
        var game = new Monopoly("Test", map);
        var playerA = new Player("A", 5000);
        game.AddPlayer(playerA);
        game.Initial();

        // Act
        // 玩家B進行購買F4
        Assert.ThrowsException<Exception>(() => game.BuyLand(playerA, "F4"), "必須在購買的土地上才可以購買");

        // Assert
        // 玩家A持有金額為5000
        // 玩家A無持有的房地產
        Assert.AreEqual(5000, playerA.Money);
        Assert.AreEqual(0, playerA.LandContractList.Count());
    }
}