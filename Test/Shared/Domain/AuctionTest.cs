namespace SharedTests.Domain;

using Shared.Domain;
using Shared.Domain.Exceptions;
using SharedTests;

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
        var game = 玩家A持有1000元_玩家B持有2000元_玩家A擁有A1_玩家A正在拍賣A1(out var a, out var b);

        // Act
        game.EndAuction();

        // Assert
        Assert.IsNull(a.FindLandContract("A1"));
        Assert.AreEqual(a.Money, 1700);
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
        var game = 玩家A持有1000元_玩家B持有2000元_玩家A擁有A1_玩家A正在拍賣A1(out var a, out var b);
        game.PlayerBid(b.Id, 600);

        // Act
        game.EndAuction();

        // Assert
        Assert.IsNull(a.FindLandContract("A1"));
        Assert.AreEqual(a.Money, 1600);
        Assert.AreEqual(b.Money, 1400);
        Assert.IsNotNull(b.FindLandContract("A1"));
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
        var game = 玩家A持有1000元_玩家B持有2000元_玩家A擁有A1_玩家A正在拍賣A1(out var a, out var b);

        // Act
        Assert.ThrowsException<BidException>(() => game.PlayerBid(b.Id, 3000));
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
        var game = 玩家A持有1000元_玩家B持有2000元_玩家A擁有A1_玩家A正在拍賣A1(out var a, out var b);
        b.Money = 3000;

        // Act
        game.PlayerBid(b.Id, 3000);
    }

    private static Game 玩家A持有1000元_玩家B持有2000元_玩家A擁有A1_玩家A正在拍賣A1(out Player a, out Player b)
    {
        var map = new Map(Utils.SevenXSevenMap());
        Game game = new("g1", map);
        a = new("A", 1000);
        b = new("B", 2000);
        game.AddPlayer(a);
        game.AddPlayer(b);
        game.Initial();
        Land A1 = (Land)map.FindBlockById("A1");
        a.AddLandContract(new(a, A1));
        game.PlayerSellLandContract(a.Id, "A1");
        return game;
    }
}