using Shared.Domain.Exceptions;
using Shared.Domain;
using static Shared.Domain.Map;

namespace SharedTests.Domain;

[TestClass]
public class RollDiceTest
{
    [TestMethod]
    [Description(
        """
        Given:  目前玩家在F4
        When:   玩家擲骰得到7點
        Then:   A 移動到 A4
        """)]
    public void 玩家擲骰後移動棋子()
    {
        // Arrange
        var map = new Map(Utils.SevenXSevenMap());
        // 灌了水銀的骰子 1 顆, 只會骰 6 點
        var game = new Game("Test", map, Utils.MockDice(6));
        var player = new Player("A");
        game.AddPlayer(player);
        game.Initial();
        game.SetPlayerToBlock(player, "F4", Direction.Up);

        // Act
        game.PlayerRollDice(player.Id);

        // Assert
        Assert.AreEqual("A4", game.GetPlayerPosition("A").Id);
    }

    [TestMethod]
    [Description(
        """
        Given:  目前玩家在F4
        When:   玩家擲骰得到7點
        Then:   A 移動到 停車場
                玩家需要選擇方向
        """)]
    public void 玩家擲骰後移動棋子到需要選擇方向的地方()
    {
        // Arrange
        var map = new Map(Utils.SevenXSevenMap());
        var game = new Game("Test", map, Utils.MockDice(1, 6));
        var player = new Player("A");
        game.AddPlayer(player);
        game.Initial();
        game.SetPlayerToBlock(player, "F4", Direction.Up);

        // Act
        Assert.ThrowsException<PlayerNeedToChooseDirectionException>(() => game.PlayerRollDice(player.Id));

        //Assert
        Assert.AreEqual("ParkingLot", game.GetPlayerPosition("A").Id);
    }
}
