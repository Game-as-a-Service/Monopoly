using Application.Common;
using Domain;
using Domain.Events;
using Domain.Maps;
using Microsoft.AspNetCore.SignalR.Client;

namespace ServerTests.AcceptanceTests;

[TestClass]
public class RollDiceTest
{
    private MonopolyTestServer server;

    [TestInitialize]
    public void Setup()
    {
        server = new MonopolyTestServer();
    }

    [TestMethod]
    [Description(
        """
        Given:  目前玩家在F4
        When:   玩家擲骰得到7點
        Then:   A 移動到 A4
        """)]
    public async Task 玩家擲骰後移動棋子()
    {
        // Arrange
        Player A = new("A");
        SetupMonopoly("1", new[] { A }, new[] { 7 });

        var hub = server.CreateHubConnection();
        var verifyPlayerRollDiceEvent = hub.Verify<PlayerRollDiceEvent>("PlayerRollDiceEvent");
        var verifyChessMoveEvent = hub.Verify<ChessMoveEvent>("ChessMoveEvent");

        // Act
        await hub.SendAsync("PlayerRollDice", "1", "A");

        // Assert
        await verifyPlayerRollDiceEvent.Verify(e => e.GameId == "1"
                                                    && e.PlayerId == "A"
                                                    && e.DiceCount == 7);
        await verifyChessMoveEvent.Verify(e => e.GameId == "1"
                                               && e.PlayerId == "A"
                                               && e.BlockId == "A4");
    }

    private void SetupMonopoly(string gameId, Player[] players, int[] dices)
    {
        var repo = server.GetRequiredService<IRepository>();
        var game = new Monopoly(gameId, new SevenXSevenMap(), Utils.MockDice(dices));
        foreach (var player in players)
        {
            game.AddPlayer(player);
        }
        game.Initial();
        repo.Save(game);
    }
}