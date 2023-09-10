using Domain.Builders;
using Domain.Maps;

namespace DomainTests.Testcases;

[TestClass]
public class BankruptTest
{
    [TestMethod]
    public void 玩家A_A沒錢沒房__更新玩家A的狀態__玩家A的狀態為破產()
    {
        // Arrange
        Map map = new SevenXSevenMap();
        var A = new { Id = "a", Money = 0m, CurrentBlockId = "Start", CurrentDirection = "Right" };

        var player_a = new PlayerBuilder(A.Id)
            .WithMoney(A.Money)
            .WithMap(map)
            .WithPosition(A.CurrentBlockId, A.CurrentDirection)
            .Build();

        var monopoly = new MonopolyBuilder()
            .WithMap(map)
            .WithPlayer(player_a)
            .Build();
        monopoly.Initial();

        // Act
        monopoly.UpdatePlayerState(player_a);

        // Assert
        Assert.AreEqual(PlayerState.Bankrupt, player_a.State);
    }
}