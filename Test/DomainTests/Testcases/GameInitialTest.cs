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
        var monopoly = new MonopolyBuilder()
            .WithMap(map)
            .WithPlayer(A.Id)
            .WithPlayer(B.Id)
            .WithPlayer(C.Id)
            .WithPlayer(D.Id)
            .WithCurrentPlayer(A.Id)
            .Build();

        // Act
        monopoly.Initial();

        // Assert
        var player_a = monopoly.Players.First(p => p.Id == A.Id);
        var player_b = monopoly.Players.First(p => p.Id == B.Id);
        var player_c = monopoly.Players.First(p => p.Id == C.Id);
        var player_d = monopoly.Players.First(p => p.Id == D.Id);
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