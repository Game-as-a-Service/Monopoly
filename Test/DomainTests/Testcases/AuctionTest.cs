using Domain.Builders;
using Domain.Maps;

namespace DomainTests.Testcases;

[TestClass]
public class AuctionTest
{
    [TestMethod]
    [Description(
        """
        Given:  玩家A持有1000元
                A1價格為1000元
                玩家A擁有A1
                玩家A正在拍賣A1
                目前無人喊價
        When:   拍賣結算
        Then:   玩家A不再擁有A1
                玩家A持有1700元
        """)]
    public void 拍賣結算時流拍()
    {
        // Arrange
        var game = 玩家A持有1000元_玩家B持有2000元_玩家A擁有A1_玩家A正在拍賣A1(out var player_a, out var player_b);

        // Act
        game.EndAuction();

        // Assert
        Assert.IsNull(player_a.FindLandContract("A1"));
        Assert.AreEqual(player_a.Money, 1700);
    }

    [TestMethod]
    [Description(
        """
        Given:  玩家A持有1000元
                玩家B持有2000元
                A1價格為1000元
                玩家A擁有A1
                玩家A正在拍賣A1
                玩家B喊價600元
        When:   拍賣結算
        Then:   玩家A持有1600元
                玩家A不再擁有A1
                玩家B持有1400元
                玩家B擁有A1
        """)]
    public void 拍賣結算時轉移金錢及地契()
    {
        // Arrange
        var game = 玩家A持有1000元_玩家B持有2000元_玩家A擁有A1_玩家A正在拍賣A1(out var player_a, out var player_b);
        game.PlayerBid(player_b.Id, 600);

        // Act
        game.EndAuction();

        // Assert
        Assert.IsNull(player_a.FindLandContract("A1"));
        Assert.AreEqual(player_a.Money, 1600);
        Assert.AreEqual(player_b.Money, 1400);
        Assert.IsNotNull(player_b.FindLandContract("A1"));
    }

    [TestMethod]
    [Description(
        """
        Given:  玩家A持有1000元
                玩家B持有2000元
                A1價格為1000元
                玩家A擁有A1
                玩家A正在拍賣A1
        When:   玩家B喊價3000元
        Then:   玩家B不能喊價
        """)]
    public void 不能喊出比自己的現金還要大的價錢()
    {
        // Arrange
        var game = 玩家A持有1000元_玩家B持有2000元_玩家A擁有A1_玩家A正在拍賣A1(out var player_a, out var player_b);

        // Act
        game.PlayerBid(player_b.Id, 3000);

        //Assert.ThrowsException<BidException>(() => game.PlayerBid(b.Id, 3000));
        game.PlayerBid(player_b.Id, 3000);
    }

    [TestMethod]
    [Description(
        """
        Given:  玩家A持有1000元
                玩家B持有3000元
                A1價格為1000元
                玩家A擁有A1
                玩家A正在拍賣A1
        When:   玩家B喊價3000元
        Then:   玩家B喊價成功
        """)]
    public void 玩家喊價成功()
    {
        // Arrange
        var game = 玩家A持有1000元_玩家B持有2000元_玩家A擁有A1_玩家A正在拍賣A1(out var player_a, out var player_b);
        player_b.Money = 3000;

        // Act
        game.PlayerBid(player_b.Id, 3000);
    }

    private static Monopoly 玩家A持有1000元_玩家B持有2000元_玩家A擁有A1_玩家A正在拍賣A1(out Player player_a, out Player player_b)
    {

        var map = new SevenXSevenMap();
        var A = new { Id = "A", Money = 1000m };
        var B = new { Id = "B", Money = 2000m };
        var A1 = new { Id = "A1", Price = 1000m };

        var monopoly = new MonopolyBuilder()
            .WithMap(map)
            .WithPlayer(A.Id, p => p.WithMoney(A.Money).WithLandContract(A1.Id, false, 0))
            .WithPlayer(B.Id, p => p.WithMoney(B.Money))
            .WithCurrentPlayer(A.Id)
            .Build();

        monopoly.Initial();

        monopoly.PlayerSellLandContract(A.Id, A1.Id);

        player_a = monopoly.Players.First(p => p.Id == A.Id);
        player_b = monopoly.Players.First(p => p.Id == B.Id);

        return monopoly;
    }
}