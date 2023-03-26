using Moq;
using Server.Hubs;
using SignalR_UnitTestingSupportCommon.Hubs;

namespace ServerTests.HubTests;

[TestClass]
public class MonopolyHubTest
{
    [TestMethod]
    [Ignore]
    public async Task 輪到玩家A_玩家A在起點__玩家擲骰子得到6__所有玩家都接收到玩家A所擲的骰子點數6()
    {
        // Arrange
        var support = new HubUnitTestsSupport();
        support.SetUp();
        var hub = new MonopolyHub();
        support.AssignToHubRequiredProperties(hub);

        // Act
        await hub.PlayerRollDice("g1", "p1", null);

        // Assert
        support
        .ClientsGroupMock
        .Verify(
           x => x.SendCoreAsync(
                nameof(MonopolyHub.PlayerRollDice),
                new object[] { "p1", 6 },
                It.IsAny<CancellationToken>())
        );
    }
}