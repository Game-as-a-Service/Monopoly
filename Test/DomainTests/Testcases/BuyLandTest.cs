using Domain.Builders;
using Domain.Maps;

namespace DomainTests.Testcases;

[TestClass]
public class BuyLandTest
{
    private readonly Map map = new SevenXSevenMap();

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
        var A = new { Id = "A", Money = 5000m, CurrentBlockId = "F4", CurrentDirection = "Up" };

        var player_a = new PlayerBuilder(A.Id)
           .WithMap(map)
           .WithMoney(A.Money)
           .WithPosition(A.CurrentBlockId, A.CurrentDirection)
           .Build();

        var monopoly = new MonopolyBuilder()
            .WithMap(map)
            .WithPlayer(player_a)
            .Build();
        monopoly.Initial();

        // Act
        // 玩家A進行購買F4
        monopoly.BuyLand(player_a.Id, "F4");

        // Assert
        // 玩家A持有金額為4000
        // 玩家A持有的房地產 F4
        Assert.AreEqual(4000, player_a.Money);
        Assert.AreEqual(1, player_a.LandContractList.Count());
        Assert.IsTrue(player_a.LandContractList.Any(pa => pa.Land.Id == "F4"));
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
        var A = new { Id = "A", Money = 500m, CurrentBlockId = "F4", CurrentDirection = "Up" };

        var player_a = new PlayerBuilder(A.Id)
           .WithMap(map)
           .WithMoney(A.Money)
           .WithPosition(A.CurrentBlockId, A.CurrentDirection)
           .Build();

        var monopoly = new MonopolyBuilder()
            .WithMap(map)
            .WithPlayer(player_a)
            .Build();
        monopoly.Initial();

        // Act
        monopoly.BuyLand(player_a.Id, "F4");

        // Assert
        // 玩家A持有金額為500
        // 玩家A無持有的房地產
        Assert.AreEqual(500, player_a.Money);
        Assert.AreEqual(0, player_a.LandContractList.Count);
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
        var A = new { Id = "A", Money = 5000m, CurrentBlockId = "F4", CurrentDirection = "Up" };
        var B = new { Id = "B", Money = 5000m };
        var F4 = new { Id = "F4", Price = 1000m };

        var player_a = new PlayerBuilder(A.Id)
           .WithMap(map)
           .WithMoney(A.Money)
           .WithPosition(A.CurrentBlockId, A.CurrentDirection)
           .Build();

        var player_b = new PlayerBuilder(B.Id)
           .WithMap(map)
           .WithMoney(B.Money)
           .WithLandContract(F4.Id)
           .Build();

        var monopoly = new MonopolyBuilder()
            .WithMap(map)
            .WithPlayer(player_a)
            .Build();
        monopoly.Initial();

        // Act
        // 玩家B進行購買F4
        monopoly.BuyLand(player_a.Id, F4.Id);

        // Assert
        // 玩家A持有金額為5000
        // 玩家A無持有的房地產
        Assert.AreEqual(5000, player_a.Money);
        Assert.AreEqual(0, player_a.LandContractList.Count);
        // 玩家B持有金額為5000
        // 玩家B持有的房地產F4
        Assert.AreEqual(5000, player_b.Money);
        Assert.AreEqual(1, player_b.LandContractList.Count);
        Assert.IsTrue(player_b.LandContractList.Any(pa => pa.Land.Id == F4.Id));
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
        var A = new { Id = "A", Money = 5000m, CurrentBlockId = "F2", CurrentDirection = "Up" };
        var F4 = new { Id = "F4", Price = 1000m };

        var player_a = new PlayerBuilder(A.Id)
           .WithMap(map)
           .WithMoney(A.Money)
           .WithPosition(A.CurrentBlockId, A.CurrentDirection)
           .Build();

        var monopoly = new MonopolyBuilder()
            .WithMap(map)
            .WithPlayer(player_a)
            .Build();
        monopoly.Initial();

        // Act
        // 玩家B進行購買F4
        monopoly.BuyLand(player_a.Id, F4.Id);

        // Assert
        // 玩家A持有金額為5000
        // 玩家A無持有的房地產
        Assert.AreEqual(5000, player_a.Money);
        Assert.AreEqual(0, player_a.LandContractList.Count);
    }
}