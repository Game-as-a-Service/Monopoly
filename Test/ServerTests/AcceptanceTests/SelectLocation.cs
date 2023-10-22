using Server.Hubs;
using SharedLibrary;
using static ServerTests.Utils;

namespace ServerTests.AcceptanceTests;

[TestClass]
public class SelectLocation
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
        Given:  玩家A:未選擇位置
        When:   玩家A選擇位置紅(位置ID=1)
        Then:   玩家A的位置為1
        """)]
    public async Task 玩家選擇房間位置()
    {
        //Arrange
        var A = new { Id = "A", locationId = 1 };

        var monopolyBuilder = new MonopolyBuilder("1")
            .WithPlayer(
                new PlayerBuilder("A")
                .Build()
            );
        monopolyBuilder.Save(server);

        var hub = await server.CreateHubConnectionAsync(gameId, "A");

        //Act
        await hub.SendAsync(nameof(MonopolyHub.PlaySelectRoomLocation), gameId, A.Id, A.locationId);

        //Assert
        hub.Verify<string, int>(
            nameof(IMonopolyResponses.PlaySelectRoomLocationEvent),
            (playerId, locationId)
                                  => playerId == A.Id && locationId == A.locationId);
        hub.VerifyNoElseEvent();
    }
}
