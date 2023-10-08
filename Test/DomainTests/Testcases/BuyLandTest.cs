using Domain.Builders;
using Domain.Events;
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

        var monopoly = new MonopolyBuilder()
            .WithMap(map)
            .WithPlayer(A.Id, pa => pa.WithMoney(A.Money).WithPosition(A.CurrentBlockId, A.CurrentDirection))
            .WithCurrentPlayer(A.Id)
            .Build();

        // Act
        // 玩家A進行購買F4
        monopoly.BuyLand(A.Id, "F4");

        // Assert
        // 玩家A持有金額為4000
        // 玩家A持有的房地產 F4
        var player_a = monopoly.Players.First(p => p.Id == A.Id);
        Assert.AreEqual(4000, player_a.Money);
        Assert.AreEqual(1, player_a.LandContractList.Count());
        Assert.IsTrue(player_a.LandContractList.Any(pa => pa.Land.Id == "F4"));

        monopoly.DomainEvents
            .NextShouldBe(new PlayerBuyBlockEvent(A.Id, "F4"))
            .NoMore();
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

        var monopoly = new MonopolyBuilder()
            .WithMap(map)
            .WithPlayer(A.Id, pa => pa.WithMoney(A.Money)
                                   .WithPosition(A.CurrentBlockId, A.CurrentDirection))
            .WithCurrentPlayer(A.Id)
            .Build();

        // Act
        monopoly.BuyLand(A.Id, "F4");

        // Assert
        // 玩家A持有金額為500
        // 玩家A無持有的房地產
        var player_a = monopoly.Players.First(p => p.Id == A.Id);
        Assert.AreEqual(500, player_a.Money);
        Assert.AreEqual(0, player_a.LandContractList.Count);

        monopoly.DomainEvents
            .NextShouldBe(new PlayerBuyBlockInsufficientFundsEvent(A.Id, "F4", 1000))
            .NoMore();
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

        var monopoly = new MonopolyBuilder()
            .WithMap(map)
            .WithPlayer(A.Id, pa => pa.WithMoney(A.Money)
                                      .WithPosition(A.CurrentBlockId, A.CurrentDirection))
            .WithPlayer(B.Id, pb => pb.WithMoney(B.Money)
                                      .WithLandContract(F4.Id, false, 0))
            .WithCurrentPlayer(A.Id)
            .Build();

        // Act
        // 玩家B進行購買F4
        monopoly.BuyLand(A.Id, F4.Id);

        // Assert
        // 玩家A持有金額為5000
        // 玩家A無持有的房地產
        var player_a = monopoly.Players.First(p => p.Id == A.Id);
        Assert.AreEqual(5000, player_a.Money);
        Assert.AreEqual(0, player_a.LandContractList.Count);
        // 玩家B持有金額為5000
        // 玩家B持有的房地產F4
        var player_b = monopoly.Players.First(p => p.Id == B.Id);
        Assert.AreEqual(5000, player_b.Money);
        Assert.AreEqual(1, player_b.LandContractList.Count);
        Assert.IsTrue(player_b.LandContractList.Any(pa => pa.Land.Id == F4.Id));

        monopoly.DomainEvents
            .NextShouldBe(new PlayerBuyBlockOccupiedByOtherPlayerEvent(A.Id, F4.Id))
            .NoMore();
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

        var monopoly = new MonopolyBuilder()
            .WithMap(map)
            .WithPlayer(A.Id, pa => pa.WithMoney(A.Money)
                                      .WithPosition(A.CurrentBlockId, A.CurrentDirection))
            .WithCurrentPlayer(A.Id)
            .Build();

        // Act
        // 玩家B進行購買F4
        monopoly.BuyLand(A.Id, F4.Id);

        // Assert
        // 玩家A持有金額為5000
        // 玩家A無持有的房地產
        var player_a = monopoly.Players.First(p => p.Id == A.Id);
        Assert.AreEqual(5000, player_a.Money);
        Assert.AreEqual(0, player_a.LandContractList.Count);

        monopoly.DomainEvents
            .NextShouldBe(new PlayerBuyBlockMissedLandEvent(A.Id, F4.Id))
            .NoMore();
    }
}