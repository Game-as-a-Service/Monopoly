using Domain.Common;

namespace Domain.Events;

public record PlayerRolledDiceEvent(string GameId, string PlayerId, int DiceCount) : DomainEvent(GameId);