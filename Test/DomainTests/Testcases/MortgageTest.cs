using Domain.Builders;
using Domain.Events;
using Domain.Maps;

namespace DomainTests.Testcases;

[TestClass]
public class MortgageTest
{
    private static Map Map => new SevenXSevenMap();

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
        var A = new { Id = "A", Money = 5000m };
        var A1 = new { Id = "A1", Price = 1000m };

        var map = Map;

        var monopoly = new MonopolyBuilder()
            .WithMap(map)
            .WithPlayer(A.Id, a => a.WithMoney(A.Money).WithLandContract(A1.Id, false, 0))
            .WithCurrentPlayer(A.Id)
            .Build();

        // Act
        monopoly.MortgageLandContract("A", "A1");

        // Assert
        Player player_a = monopoly.Players.First(p => p.Id == A.Id);
        Assert.AreEqual(5700, player_a.Money);
        Assert.AreEqual(1, player_a.LandContractList.Count);
        Assert.IsTrue(player_a.LandContractList.Any(l => l.Land.Id == A1.Id));

        monopoly.DomainEvents
            .NextShouldBe(new PlayerMortgageEvent(A.Id, 5700, A1.Id, 10))
            .NoMore();
    }

    [TestMethod]
    [Description(
        """
        Given:  A 持有 A1，價值 1000元，抵押中
                A 持有 1000元
        When:   A 抵押 A1
        Then:   A 無法再次抵押
                A 持有 1000元
        """)]
    public void 玩家不能抵押已抵押房地產()
    {
        // Arrange
        var A = new { Id = "A", Money = 1000m };
        var A1 = new { Id = "A1", Price = 1000m, IsMortgage = true };

        var map = Map;

        var monopoly = new MonopolyBuilder()
            .WithMap(map)
            .WithPlayer(A.Id, a => a.WithMoney(A.Money).WithLandContract(A1.Id, A1.IsMortgage, 5))
            .WithCurrentPlayer(A.Id)
            .Build();

        // Act
        monopoly.MortgageLandContract("A", "A1");

        // Assert
        var player_a = monopoly.Players.First(p => p.Id == A.Id);
        Assert.AreEqual(1, player_a.LandContractList.Count);
        Assert.IsTrue(player_a.LandContractList.Any(l => l.Land.Id == A1.Id));
        Assert.AreEqual(A.Money, player_a.Money);

        monopoly.DomainEvents
            .NextShouldBe(new PlayerCannotMortgageEvent(A.Id, 1000, A1.Id))
            .NoMore();
    }

    [TestMethod]
    [Description(
        """
        Given:  A 持有 5000元
        When:   A 抵押 A1
        Then:   A 抵押 失敗
        """)]
    public void 玩家抵押非自有房地產()
    {
        // Arrange
        var A = new { Id = "A", Money = 5000m };
        var A1 = new { Id = "A1", Price = 1000m };


        var map = Map;

        var monopoly = new MonopolyBuilder()
            .WithMap(map)
            .WithPlayer(A.Id, a => a.WithMoney(A.Money))
            .WithCurrentPlayer(A.Id)
            .Build();

        // Act
        monopoly.MortgageLandContract("A", A1.Id);

        // Assert
        var player_a = monopoly.Players.First(p => p.Id == A.Id);
        Assert.AreEqual(0, player_a.LandContractList.Count);
        Assert.IsFalse(player_a.LandContractList.Any(l => l.Land.Id == A1.Id));
        Assert.AreEqual(A.Money, player_a.Money);

        monopoly.DomainEvents
            .NextShouldBe(new PlayerCannotMortgageEvent(A.Id, 5000, A1.Id))
            .NoMore();
    }
}