using Domain.Common;

namespace Domain.Events;

public record PlayerRollDiceEvent(string GameId, string PlayerId, int DiceCount) : DomainEvent(GameId);