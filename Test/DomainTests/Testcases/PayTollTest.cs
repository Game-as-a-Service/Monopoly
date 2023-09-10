using Domain.Builders;
using Domain.Events;
using Domain.Maps;

namespace DomainTests.Testcases;

[TestClass]
public class PayTollTest
{
    private static Map Map => new SevenXSevenMap();

    [TestMethod]
    [Description(
        """
        Given:  玩家A, B
                玩家A持有的金額 1000, 房地產 A4, 地價 1000, 沒有房子
                玩家B持有的金額 1000
                B的回合移動到A4
        When:   B扣除過路費1000 * 5% = 50給A
        Then:   玩家A持有金額為1000+50 = 1050
                玩家A持有金額為1000-50 = 950
        """)]
    public void 玩家付過路費_無房_無同地段()
    {
        // Arrange
        // 玩家AB
        // A餘額1000, B餘額1000
        var A = new { Id = "A", Money = 1000m };
        var B = new { Id = "B", CurrentBlockId = "A4", CurrentDirection = "Down", Money = 1000m };
        var A4 = new { Id = "A4", Price = 1000m };

        var map = Map;

        var monopoly = new MonopolyBuilder()
            .WithMap(map)
            .WithPlayer(A.Id, a => a.WithMoney(A.Money).WithLandContract(A4.Id, false, 0))
            .WithPlayer(B.Id, b => b.WithMoney(B.Money).WithPosition(B.CurrentBlockId, B.CurrentDirection))
            .WithCurrentPlayer(B.Id)
            .Build();

        //Act
        monopoly.PayToll(B.Id);

        // Assert
        // 1000 * 0.05 = 50
        monopoly.DomainEvents
            .NextShouldBe(new PlayerPayTollEvent(B.Id, 950, A.Id, 1050))
            .NoMore();
    }

    [TestMethod]
    [Description(
        """
        Given:  玩家A, B
                玩家A持有的金額 1000, 房地產 A1 A4, A4地價 1000, A4有2棟房子
                玩家B持有的金額 2000
                B的回合移動到A4
        When:   B扣除過路費1000 * 100% * 130% = 1300給A
        Then:   玩家A持有金額為1000+1300 = 2300
                玩家A持有金額為2000-1300 = 700
        """)]
    public void 玩家付過路費_有2房_有1同地段()
    {
        // Arrange
        var A = new { Id = "A", Money = 1000m };
        var B = new { Id = "B", CurrentBlockId = "A4", CurrentDirection = "Down", Money = 2000m };
        var A1 = new { Id = "A1", Price = 1000m };
        var A4 = new { Id = "A4", Price = 1000m, HouseCount = 2 };

        var map = Map;
        map.FindBlockById<Land>(A4.Id).Upgrade();
        map.FindBlockById<Land>(A4.Id).Upgrade();

        var monopoly = new MonopolyBuilder()
            .WithMap(map)
            .WithPlayer(A.Id, a => a.WithMoney(A.Money)
                                    .WithLandContract(A1.Id, false, 10)
                                    .WithLandContract(A4.Id, false, 0))
            .WithPlayer(B.Id, b => b.WithMoney(B.Money).WithPosition(B.CurrentBlockId, B.CurrentDirection))
            .WithCurrentPlayer(B.Id)
            .Build();

        // Act
        monopoly.PayToll(B.Id);

        // 1000 * 100% * 130% = 1300
        monopoly.DomainEvents
            .NextShouldBe(new PlayerPayTollEvent(B.Id, 700, A.Id, 2300))
            .NoMore();
    }

    [TestMethod]
    [Description(
        """
        Given:  玩家A, B
                目前輪到A
                A在A1上，持有1000元
                B持有1000元
                B在監獄(Jail)
                A1是B的土地，價值1000元
        When:   A付過路費
        Then:   A無須付過路費
        """)]
    public void 地主在監獄無需付過路費()
    {
        // Arrange
        var A = new { Id = "A", Money = 1000m, CurrentBlockId = "A1", CurrentDirection = "Right" };
        var B = new { Id = "B", Money = 1000m, CurrentBlockId = "Jail", CurrentDirection = "Down" };
        var A1 = new { Id = "A1", Price = 1000m };

        var map = Map;

        var monopoly = new MonopolyBuilder()
            .WithMap(map)
            .WithPlayer(A.Id, a => a.WithMoney(A.Money).WithPosition(A.CurrentBlockId, A.CurrentDirection))
            .WithPlayer(B.Id, b => b.WithMoney(B.Money)
                                    .WithPosition(B.CurrentBlockId, B.CurrentDirection)
                                    .WithLandContract(A1.Id, false, 0))
            .WithCurrentPlayer(A.Id)
            .Build();

        // Act
        monopoly.PayToll(A.Id);

        // Assert
        Assert.AreEqual(1000, A.Money);
        Assert.AreEqual(1000, B.Money);

        monopoly.DomainEvents
            .NextShouldBe(new PlayerDoesntNeedToPayTollEvent(A.Id, 1000))
            .NoMore();
    }

    [TestMethod]
    [Description(
        """
        Given:  玩家A, B
                目前輪到A
                A在A1上，持有1000元
                B持有1000元
                B在停車場
                A1是B的土地，價值1000元
        When:   A付過路費
        Then:   A無須付過路費
        """)]
    public void 地主在停車場無需付過路費()
    {
        // Arrange
        var A = new { Id = "A", Money = 1000m, CurrentBlockId = "A1", CurrentDirection = "Right" };
        var B = new { Id = "B", Money = 1000m, CurrentBlockId = "ParkingLot", CurrentDirection = "Down" };
        var A1 = new { Id = "A1", Price = 1000m };

        var map = Map;

        var monopoly = new MonopolyBuilder()
            .WithMap(map)
            .WithPlayer(A.Id, a => a.WithMoney(A.Money).WithPosition(A.CurrentBlockId, A.CurrentDirection))
            .WithPlayer(B.Id, b => b.WithMoney(B.Money)
                                    .WithPosition(B.CurrentBlockId, B.CurrentDirection)
                                    .WithLandContract(A1.Id, false, 0))
            .WithCurrentPlayer(A.Id)
            .Build();

        // Act
        monopoly.PayToll(A.Id);

        // Assert
        monopoly.DomainEvents
            .NextShouldBe(new PlayerDoesntNeedToPayTollEvent(A.Id, 1000))
            .NoMore();
    }

    [TestMethod]
    [Description(
    """
        Given:  玩家A, B
                玩家A持有的金額 1000, 房地產 A1 A4, A4地價 1000, A4有2棟房子
                玩家B持有的金額 2000
                B的回合移動到A4
                B已支付過路費
        Wheh:   B再支付一次過路費
        Then:   B第二次支付失敗
                玩家B持有金額為2000
        """)]
    public void 玩家不能重複支付過路費()
    {
        // Arrange
        var A = new { Id = "A", Money = 1000m };
        var B = new { Id = "B", CurrentBlockId = "A4", CurrentDirection = "Down", Money = 2000m };
        var A1 = new { Id = "A1", Price = 1000m };
        var A4 = new { Id = "A4", Price = 1000m, HouseCount = 2 };

        var map = Map;
        map.FindBlockById<Land>(A4.Id).Upgrade();
        map.FindBlockById<Land>(A4.Id).Upgrade();

        var monopoly = new MonopolyBuilder()
            .WithMap(map)
            .WithPlayer(A.Id, a => a.WithMoney(A.Money)
                                    .WithLandContract(A1.Id, false, 0)
                                    .WithLandContract(A4.Id, false, 0))
            .WithPlayer(B.Id, b => b.WithMoney(B.Money).WithPosition(B.CurrentBlockId, B.CurrentDirection))
            .WithCurrentPlayer(B.Id, p => p.WithPayToll())
            .Build();

        // Act
        monopoly.PayToll(B.Id);

        // 1000 * 100% * 130% = 1300
        monopoly.DomainEvents
            .NextShouldBe(new PlayerDoesntNeedToPayTollEvent(B.Id, B.Money))
            .NoMore();
    }
}