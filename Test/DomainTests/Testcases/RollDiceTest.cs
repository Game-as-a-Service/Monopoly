using Domain.Builders;
using Domain.Events;
using Domain.Maps;
using static Domain.Map;

namespace DomainTests.Testcases;

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
        var map = new SevenXSevenMap();
        // 灌了水銀的骰子 1 顆, 只會骰 6 點
        var game = new Monopoly("Test", map, Utils.MockDice(6));
        var player = new Player("A");
        game.AddPlayer(player, "F4", Direction.Up);
        game.Initial();

        // Act
        game.PlayerRollDice(player.Id);

        // Assert
        Assert.AreEqual("A4", game.GetPlayerPosition("A").Id);
    }

    [TestMethod]
    [Description(
        """
        Given:  目前玩家在F4
        When:   玩家擲骰得到8點
        Then:   A 移動到 停車場
                玩家需要選擇方向
                玩家剩餘步數為 1
        """)]
    public void 玩家擲骰後移動棋子到需要選擇方向的地方()
    {
        // Arrange
        var map = new SevenXSevenMap();
        var game = new Monopoly("Test", map, Utils.MockDice(2, 6));
        var player = new Player("A");
        game.AddPlayer(player, "F4", Direction.Up);
        game.Initial();

        // Act
        game.PlayerRollDice(player.Id);

        //Assert
        Assert.AreEqual("ParkingLot", game.GetPlayerPosition("A").Id);
        Assert.AreEqual(1, player.Chess.RemainingSteps);
        Assert.IsTrue(game.DomainEvents.Any(
            e => e is PlayerNeedToChooseDirectionEvent @event &&
                            @event.PlayerId == player.Id &&
                            @event.Directions.Count() == 3
                            ));
    }

    [TestMethod]
    [Description(
        """
        Given:  目前玩家在F3
                玩家持有1000元
        When:   玩家擲骰得到4點
        Then:   玩家移動到 A1
                玩家剩餘步數為 0
                玩家持有4000元
        """)]
    public void 玩家擲骰後移動棋子經過起點獲得獎勵金3000()
    {
        // Arrange
        var map = new SevenXSevenMap();
        var game = new Monopoly("Test", map, Utils.MockDice(2, 2));
        var player = new Player("A", 1000);
        game.AddPlayer(player, "F3", Direction.Up);
        game.Initial();

        // Act
        game.PlayerRollDice(player.Id);

        // Assert
        Assert.AreEqual("A1", game.GetPlayerPosition("A").Id);
        Assert.AreEqual(0, player.Chess.RemainingSteps);
        Assert.AreEqual(4000, player.Money);
    }

    [TestMethod]
    [Description(
        """
        Given:  目前玩家在F3
                玩家持有1000元
        When:   玩家擲骰得到3點
        Then:   玩家移動到 起點
                玩家剩餘步數為 0
                玩家持有1000元
        """)]
    public void 玩家擲骰後移動棋子到起點無法獲得獎勵金()
    {
        // Arrange
        var map = new SevenXSevenMap();
        var game = new Monopoly("Test", map, Utils.MockDice(2, 1));
        var player = new Player("A", 1000);
        game.AddPlayer(player, "F3", Direction.Up);
        game.Initial();

        // Act
        game.PlayerRollDice(player.Id);

        // Assert
        Assert.AreEqual("Start", game.GetPlayerPosition("A").Id);
        Assert.AreEqual(0, player.Chess.RemainingSteps);
        Assert.AreEqual(1000, player.Money);
    }

    [TestMethod]
    [Description("""
        Given:  目前玩家在A1
                玩家持有A2
                A2房子不足5間
        When:   玩家擲骰得到2點
        Then:   玩家移動到 A2
                玩家剩餘步數為 0
                提示可以蓋房子
        """)]
    public void 玩家擲骰後移動棋子到自己擁有地()
    {
        // Arrange
        var map = new SevenXSevenMap();
        var game = new Monopoly("Test", map, Utils.MockDice(1, 1));
        var player = new Player("A", 1000);
        game.AddPlayer(player, "A1", Direction.Right);
        game.Initial();
        Land A2 = (Land)map.FindBlockById("A2");
        player.AddLandContract(new LandContract(player, A2));

        // Act
        game.PlayerRollDice(player.Id);

        // Assert
        Assert.AreEqual("A2", game.GetPlayerPosition("A").Id);
        Assert.AreEqual(0, player.Chess.RemainingSteps);
        Assert.IsTrue(game.DomainEvents.Any(
                       e => e is PlayerCanBuildHouseEvent @event &&
                               @event.PlayerId == player.Id &&
                               @event.BlockId == "A2" &&
                               @event.HouseCount == 0 &&
                               @event.UpgradeMoney == A2.UpgradePrice));
    }
}