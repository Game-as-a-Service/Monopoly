using Application.Common;
using Server.Hubs;

namespace ServerTests.AcceptanceTests;

[TestClass]
public class CreateGameTest
{
    private MonopolyTestServer server;
    private IRepository repository;

    [TestInitialize]
    public void Setup()
    {
        server = new MonopolyTestServer();
        repository = server.GetRequiredService<IRepository>();
    }

    [TestMethod]
    public async Task 建立遊戲()
    {
        // Arrange
        var hub = server.CreateHubConnection();

        // Act
        await hub.SendAsync(nameof(MonopolyHub.CreateGame), "a");

        // Assert
        hub.Verify<string>(nameof(IMonopolyResponses.GameCreatedEvent), GameId => GameId == "1");
    }
}