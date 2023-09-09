using Domain.Builders;
using Domain.Maps;

namespace DomainTests.Testcases;

[TestClass]
public class GameInitialTest
{
    private static Map Map => new SevenXSevenMap();

    [TestMethod]
    public void 玩家ABCD__遊戲初始設定__每位玩家有15000_每位玩家都會在起點()
    {
        // Arrange
        var A = new { Id = "A" };
        var B = new { Id = "B" };
        var C = new { Id = "C" };
        var D = new { Id = "D" };

        var map = Map;

        var player_a = new PlayerBuilder(A.Id)
            .WithMap(map)
            .Build();
        var player_b = new PlayerBuilder(B.Id)
            .WithMap(map)
            .Build();
        var player_c = new PlayerBuilder(C.Id)
            .WithMap(map)
            .Build();
        var player_d = new PlayerBuilder(D.Id)
            .WithMap(map)
            .Build();
        var monopoly = new MonopolyBuilder()
            .WithMap(map)
            .WithPlayer(player_a)
            .WithPlayer(player_b)
            .WithPlayer(player_c)
            .WithPlayer(player_d)
            .Build();

        // Act
        monopoly.Initial();

        // Assert
        Assert.AreEqual(15000, player_a.Money);
        Assert.AreEqual(15000, player_b.Money);
        Assert.AreEqual(15000, player_c.Money);
        Assert.AreEqual(15000, player_d.Money);

        Assert.AreEqual("Start", monopoly.GetPlayerPosition("A").Id);
        Assert.AreEqual("Start", monopoly.GetPlayerPosition("B").Id);
        Assert.AreEqual("Start", monopoly.GetPlayerPosition("C").Id);
        Assert.AreEqual("Start", monopoly.GetPlayerPosition("D").Id);
    }
}