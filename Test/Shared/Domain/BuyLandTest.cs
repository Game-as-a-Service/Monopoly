using Shared.Domain;
using static Shared.Domain.Map;

namespace SharedTests.Domain;

[TestClass]
public class BuyLandTest
{
    [TestMethod]
    public void 玩家A在空地上__購買土地__擁有土地()
    {
        // Arrange
        // 玩家A持有的金額 5000
        // 玩家A目前踩在價值為1000的土地F4上
        var map = new Map(Utils.SevenXSevenMap());
        var game = new Game("Test", map);
        var playerA = new Player("A", 5000);
        game.AddPlayer(playerA);
        game.Initial();
        game.SetPlayerToBlock(playerA, "F4", Direction.Up);

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
    [ExpectedException(typeof(Exception), "金額不足")]
    public void 玩家A在空地上__購買土地__金額不足()
    {
        // Arrange
        // 玩家A持有的金額 5000
        // 玩家A目前踩在價值為1000的土地F4上
        var map = new Map(Utils.SevenXSevenMap());
        var game = new Game("Test", map);
        var playerA = new Player("A", 500);
        game.AddPlayer(playerA);
        game.Initial();
        game.SetPlayerToBlock(playerA, "F4", Direction.Up);

        // Act
        // 玩家A進行購買F4
        game.BuyLand(playerA, "F4");

        // Assert
        // 玩家A持有金額為5000
        // 玩家A無持有的房地產
        Assert.AreEqual(5000, playerA.Money);
        Assert.AreEqual(0, playerA.LandContractList.Count());
        Assert.IsFalse(playerA.LandContractList.Any(pa => pa.Land.Id == "F4"));
    }
}