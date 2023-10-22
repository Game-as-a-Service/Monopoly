using Application.DataModels;
using Server.Hubs;
using SharedLibrary;
using static ServerTests.Utils;

namespace ServerTests.AcceptanceTests;

[TestClass]
public class SelectRoleTest
{
    private MonopolyTestServer server = default!;
    private const string gameId = "1";

    [TestInitialize]
    public void Setup()
    {
        server = new MonopolyTestServer();
    }

    [TestMethod]
    [Description("""
        Given:  玩家A在房間內，此時無人物
        When:   玩家A選擇角色Id為 "1"
        Then:   玩家A的角色為Id "1"
        """)]
    public async Task 玩家選擇角色()
    {
        // Arrange
        var monopolyBuilder = new MonopolyBuilder("1")
            .WithGameStage(GameStage.Preparing)
            .WithPlayer(
                new PlayerBuilder("A")
                .Build()
            );

        monopolyBuilder.Save(server);

        var hub = await server.CreateHubConnectionAsync(gameId, "A");

        // Act
        await hub.SendAsync(nameof(MonopolyHub.PlayerSelectRole), gameId, "A", "1");

        // Assert
        hub.Verify<string, string>(
            nameof(IMonopolyResponses.PlayerSelectRoleEvent),
                (playerId, roleId) => (playerId, roleId) == ("A", "1")
            );
    }
}
