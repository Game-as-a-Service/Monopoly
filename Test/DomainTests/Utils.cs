using Domain.Common;
using Domain.Events;
using Moq;

namespace DomainTests;

public class Utils
{
    public static IDice[] MockDice(params int[] diceValues)
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
}

static class DomainEventsExtension
{
    public static IEnumerable<DomainEvent> NextShouldBe(this IEnumerable<DomainEvent> domainEvents, DomainEvent e)
    {
        var first = domainEvents.First();
        if (first is PlayerNeedToChooseDirectionEvent playerNeedToChooseDirectionEvent)
        {
            var (PlayerId, Directions) = (((PlayerNeedToChooseDirectionEvent)e).PlayerId, ((PlayerNeedToChooseDirectionEvent)e).Directions);
            Assert.AreEqual(PlayerId, playerNeedToChooseDirectionEvent.PlayerId);
            CollectionAssert.AreEquivalent(Directions, playerNeedToChooseDirectionEvent.Directions);
        }
        else if (first is GameSettlementEvent gameSettlementEvent)
        {
            var (Rounds, Players) = (((GameSettlementEvent)e).Rounds, ((GameSettlementEvent)e).Players);
            Assert.AreEqual(Rounds, gameSettlementEvent.Rounds);
            CollectionAssert.AreEqual(Players, gameSettlementEvent.Players);
        }
        else
        {
            Assert.AreEqual(e, first);
        }
        return domainEvents.Skip(1);
    }

    public static void NoMore(this IEnumerable<DomainEvent> domainEvents)
    {
        Assert.IsFalse(domainEvents.Any(), string.Join('\n', domainEvents));
    }
}