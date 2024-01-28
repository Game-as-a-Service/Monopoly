using Application.Common;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Server.DataModels;
using SharedLibrary;
using SharedLibrary.ResponseArgs.Monopoly;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace ServerTests.AcceptanceTests;

[TestClass]
public class PlayerJoinGameTest
{
    private MonopolyTestServer server = default!;
    private IRepository repository = default!;
    private MockJwtTokenService jwtTokenService = default!;
    private JwtBearerOptions jwtBearerOptions = default!;

    [TestInitialize]
    public void Setup()
    {
        server = new MonopolyTestServer();
        jwtTokenService = server.GetRequiredService<MockJwtTokenService>();
        repository = server.GetRequiredService<IQueryRepository>();
        jwtBearerOptions = server.GetRequiredService<IOptionsMonitor<JwtBearerOptions>>().Get("Bearer");
    }

    [TestMethod]
    [Description("""
        Given:  Id為1的遊戲，裡面有玩家 A B C
        When:   玩家A建立連線到Id為1的房間
        Then:   玩家A建立連線成功
        """)]
    public async Task 玩家建立連線成功()
    {
        // Arrange
        await CreateGameAsync("A", "A", "B", "C");

        // Act
        var hub = await server.CreateHubConnectionAsync("1", "A");

        // Assert
        hub.Verify(nameof(IMonopolyResponses.PlayerJoinGameEvent),
            (PlayerJoinGameEventArgs e) => e.PlayerId == "A");
    }

    [TestMethod]
    [Description("""
        Given:  Id為1的遊戲，裡面有玩家 A B C
        When:   玩家A建立連線到Id為2的房間
        Then:   玩家A建立連線失敗
        """)]
    public async Task 玩家建立連線失敗因為遊戲不存在()
    {
        // Arrange
        await CreateGameAsync("A", "A", "B", "C");

        // Act
        VerificationHub hub = await server.CreateHubConnectionAsync("2", "A");

        // Assert
        hub.VerifyDisconnection();
    }
    /// <summary>
    /// call API POST "/" Body: <paramref name="playerIds"/>.
    /// Create game, which id = <paramref name="gameId"/>
    /// </summary>
    /// <param name="gameId">Game Id</param>
    /// <param name="playerIds">Players' Id</param>
    private async Task CreateGameAsync(string host, params string[] playerIds)
    {
        CreateGameBodyPayload bodyPayload = new(playerIds.Select(id => new Player(id, "")).ToArray());
        var jwt = jwtTokenService.GenerateJwtToken(jwtBearerOptions.Audience, host);
        server.Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwt);
        HttpResponseMessage? response = await server.Client.PostAsJsonAsync("/games", bodyPayload);
    }
}
