using Application.Common;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Server.DataModels;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace ServerTests.AcceptanceTests;

[TestClass]
public class CreateGameTest
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
        repository = server.GetRequiredService<IRepository>();
        jwtBearerOptions = server.GetRequiredService<IOptionsMonitor<JwtBearerOptions>>().Get("Bearer");
    }

    [TestMethod]
    [Description(
        """
        Given:  JWT 包含玩家 A B C，且玩家 A 為房主
                Body Payload:
                {
                    "players": [
                        {
                           "id": "idA",
                        },
                        {
                           "id": "idB",
                        }
                        ,
                        {
                           "id": "idC",
                        }
                    ]
                }
        When:   玩家 A 發送    
                POST /create-game
                Authorization: Bearer {hostJWT}
        Then:   回傳遊戲 {domain name of frontend}/{gameId}
                遊戲為 {gameId} 的遊戲有被建立，遊戲內有玩家 A B C，玩家A為房主
        """)]
    public async Task 建立遊戲成功()
    {
        // Arrange
        // call API POST "/"
        // Body: ["A", "B"]
        CreateGameBodyPayload bodyPayload = new(new Player[] {
                new("idA", "A"),
                new("idB", "B"),
                new("idC", "C")
            });

        var jwt = jwtTokenService.GenerateJwtToken(jwtBearerOptions.Audience, "idA");
        string gameId = "1";
        string expected = $"https://localhost:7047/games/{gameId}";

        // Act
        server.Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwt);
        HttpResponseMessage? response = await server.Client.PostAsJsonAsync("/games", bodyPayload);

        // Assert
        var data = await response.Content.ReadAsStringAsync();
        Assert.AreEqual(expected, data);
        Assert.IsTrue(repository.IsExist(gameId));
        var game = repository.FindGameById(gameId);
        Assert.AreEqual(3, game.Players.Count);
        Assert.AreEqual("idA", game.Host);
    }

    [TestMethod]
    public async Task 因為沒有建立遊戲失敗()
    {
        // Arrange
        CreateGameBodyPayload bodyPayload = new(new Player[] {
                new("idA", "A"),
                new("idB", "B"),
                new("idC", "C")
            });

        // Act
        HttpResponseMessage? response = await server.Client.PostAsJsonAsync("/games", bodyPayload);

        // Assert
        Assert.AreEqual(HttpStatusCode.Unauthorized, response.StatusCode);
    }
}