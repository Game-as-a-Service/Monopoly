using Domain.Interfaces;
using Moq;
using Server.Hubs;

namespace ServerTests;

public class Utils
{
    public static IDice[]? MockDice(params int[] diceValues)
    {
        var dice = new IDice[diceValues.Length];
        for (int i = 0; i < diceValues.Length; i++)
        {
            var mockDice = new Mock<IDice>();
            mockDice.Setup(x => x.Roll());
            mockDice.Setup(x => x.Value).Returns(diceValues[i]);
            dice[i] = mockDice.Object;
        }

        return dice;
    }
    internal static void VerifyChessMovedEvent(VerificationHub hub, string playerId, string blockId, string direction, int remainingSteps)
    {
        hub.Verify<string, string, string, int>(nameof(IMonopolyResponses.ChessMovedEvent), (PlayerId, BlockId, Direction, RemainingSteps) =>
            PlayerId == playerId && BlockId == blockId && Direction == direction && RemainingSteps == remainingSteps);
    }
}