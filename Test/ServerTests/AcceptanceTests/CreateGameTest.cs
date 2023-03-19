using Application.Common;
using Domain.Events;
using Microsoft.AspNetCore.SignalR.Client;

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
        var verification = hub.Verify<GameCreatedEvent>("GameCreatedEvent", Timeout: 5000);

        // Act
        await hub.SendAsync("CreateGame", "a");

        // Assert
        await verification.Verify(e => e.GameId == "1");
    }
}