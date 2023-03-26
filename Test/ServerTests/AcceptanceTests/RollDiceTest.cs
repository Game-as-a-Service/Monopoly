using Application.Common;
using Domain;
using Domain.Maps;
using Server.Hubs;
using static Domain.Map;

namespace ServerTests.AcceptanceTests;

[TestClass]
public class RollDiceTest
{
    private MonopolyTestServer server = default!;

    [TestInitialize]
    public void Setup()
    {
        server = new MonopolyTestServer();
    }

    [TestMethod]
    [Description(
        """
        Given:  目前玩家在F4
        When:   玩家擲骰得到6點
        Then:   A 移動到 A4
        """)]
    public async Task 玩家擲骰後移動棋子()
    {
        // Arrange
        Player A = new("A");
        SetupMonopoly("1", A, "F4", Direction.Up, new[] { 6 });

        var hub = server.CreateHubConnection();

        // Act
        await hub.SendAsync(nameof(MonopolyHub.PlayerRollDice), "1", "A");

        // Assert
        // A 擲了 6 點
        // A 移動到 Start，方向為 Right，剩下 5 步
        // A 移動到 A1，方向為 Right，剩下 4 步
        // A 移動到 Station1，方向為 Right，剩下 3 步
        // A 移動到 A2，方向為 Right，剩下 2 步
        // A 移動到 A3，方向為 Down，剩下 1 步
        // A 移動到 A4，方向為 Down，剩下 0 步
        hub.Verify<string, int>(
            nameof(IMonopolyResponses.PlayerRolledDiceEvent),
            (playerId, diceCount) => playerId == "A" && diceCount == 6);
        VerifyChessMovedEvent(hub, "A", "Start", "Right", 5);
        VerifyChessMovedEvent(hub, "A", "A1", "Right", 4);
        VerifyChessMovedEvent(hub, "A", "Station1", "Right", 3);
        VerifyChessMovedEvent(hub, "A", "A2", "Right", 2);
        VerifyChessMovedEvent(hub, "A", "A3", "Down", 1);
        VerifyChessMovedEvent(hub, "A", "A4", "Down", 0);
    }

    private void SetupMonopoly(string gameId, Player player, string initialBlockId, Direction initialDirection, int[] dices)
    {
        var repo = server.GetRequiredService<IRepository>();
        var game = new Monopoly(gameId, new SevenXSevenMap(), Utils.MockDice(dices));
        
        game.AddPlayer(player, initialBlockId, initialDirection);

        game.Initial();
        repo.Save(game);
    }

    static void VerifyChessMovedEvent(VerificationHub hub, string playerId, string blockId, string direction, int remainingSteps)
    {
        hub.Verify<string, string, string, int>(nameof(IMonopolyResponses.ChessMovedEvent), (PlayerId, BlockId, Direction, RemainingSteps) =>
            PlayerId == playerId && BlockId == blockId && Direction == direction && RemainingSteps == remainingSteps);
    }

}