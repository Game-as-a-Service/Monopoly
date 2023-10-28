using Application.DataModels;
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
        Given:  玩家A:位置未選擇
        When:   玩家A選擇位置1
        Then:   玩家A的位置為1
        """)]
    public async Task 玩家選擇沒人的位置()
    {
        //Arrange
        var A = new { Id = "A", locationId = 0, selectLocationId = 1 };

        var monopolyBuilder = new MonopolyBuilder("1")
            .WithGameStage(GameStage.Preparing)
            .WithPlayer(
                new PlayerBuilder("A")
                .WithLocation(A.locationId)
                .Build()
            )
            .WithMockDice(new[] { 1 });
        monopolyBuilder.Save(server);

        var hub = await server.CreateHubConnectionAsync(gameId, "A");

        //Act
        await hub.SendAsync(nameof(MonopolyHub.PlaySelectLocation), gameId, A.Id, A.selectLocationId);

        //Assert
        hub.Verify<string, int>(
            nameof(IMonopolyResponses.PlaySelectLocationEvent),
            (playerId, locationId) => (playerId, locationId) == (A.Id, A.selectLocationId)
            );
        hub.VerifyNoElseEvent();
    }

    [TestMethod]
    [Description("""
        Given:  玩家A:位置未選擇
                玩家B:位置1
        When:   玩家A選擇位置1
        Then:   玩家A的位置為未選擇
        """)]
    public async Task 玩家選擇有人的位置()
    {
        //Arrange
        var A = new { Id = "A", locationId = 0, selectLocationId = 1 };
        var B = new { Id = "B", locationId = 1 };

        var monopolyBuilder = new MonopolyBuilder("1")
            .WithGameStage(GameStage.Preparing)
            .WithPlayer(
                new PlayerBuilder(A.Id)
                .WithLocation(A.locationId)
                .Build()
            )
            .WithPlayer(
                new PlayerBuilder(B.Id)
                .WithLocation(B.locationId)
                .Build()
            )
            .WithMockDice(new[] { 1 });
        monopolyBuilder.Save(server);

        var hub = await server.CreateHubConnectionAsync(gameId, "A");

        //Act
        await hub.SendAsync(nameof(MonopolyHub.PlaySelectLocation), gameId, A.Id, A.selectLocationId);

        //Assert
        hub.Verify<string, int>(
            nameof(IMonopolyResponses.PlayCannotSelectLocationEvent),
            (playerId, locationId) => (playerId, locationId) == (A.Id, A.locationId)
            );
        hub.VerifyNoElseEvent();
    }

    [TestMethod]
    [Description("""
        Given:  玩家A:位置1
        When:   玩家A選擇位置2
        Then:   玩家A的位置為2
        """)]
    public async Task 有選位置的玩家更換到沒人的位置()
    {
        //Arrange
        var A = new { Id = "A", locationId = 1, selectLocationId = 2 };

        var monopolyBuilder = new MonopolyBuilder("1")
            .WithGameStage(GameStage.Preparing)
            .WithPlayer(
                new PlayerBuilder("A")
                .WithLocation(A.locationId)
                .Build()
            )
            .WithMockDice(new[] { 1 });
        monopolyBuilder.Save(server);

        var hub = await server.CreateHubConnectionAsync(gameId, "A");

        //Act
        await hub.SendAsync(nameof(MonopolyHub.PlaySelectLocation), gameId, A.Id, A.selectLocationId);

        //Assert
        hub.Verify<string, int>(
            nameof(IMonopolyResponses.PlaySelectLocationEvent),
            (playerId, locationId) => (playerId, locationId) == (A.Id, A.selectLocationId)
            );
        hub.VerifyNoElseEvent();
    }
}
