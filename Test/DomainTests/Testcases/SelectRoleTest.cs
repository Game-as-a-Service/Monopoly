using Domain.Builders;

namespace DomainTests.Testcases;
[TestClass]
public class SelectRoleTest
{
    [TestMethod]
    [Description("""
        Given:  玩家A在房間內，此時無人物
        When:   玩家A選擇角色Id為 "1"
        Then:   玩家A的角色為Id "1"
        """)]
    public void 玩家選擇角色()
    {
        // Arrange
        var monopoly = new MonopolyBuilder()
            .WithPlayer("A")
            .WithGameStage(GameStage.Preparing)
            .Build();

        // Act
        monopoly.SelectRole("A", "1");

        // Assert
        Assert.AreEqual("1", monopoly.Players.First(p => p.Id == "A").RoleId);

    }
}
