using Application.Common;
using Server.Hubs;
using System.Net.Http.Json;

namespace ServerTests.AcceptanceTests;

[TestClass]
public class CreateGameTest
{
    private MonopolyTestServer server;

    [TestInitialize]
    public void Setup()
    {
        server = new MonopolyTestServer();
    }

    [TestMethod]
    public async Task 建立遊戲()
    {
        // Arrange
        // call API POST "/"
        // Body: ["A", "B"]
        var gameId = "1";
        string expected = $"https://localhost:7047/{gameId}";
        string[] jsonContent = new[] { "A", "B" };

        // Act
        var response = await server.Client.PostAsJsonAsync("/", jsonContent);

        // Assert
        var data = await response.Content.ReadAsStringAsync();
        Assert.AreEqual(expected, data);
    }
}