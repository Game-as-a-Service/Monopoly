using Domain.Common;

namespace Domain.Events;

public record PlayerRolledDiceEvent(string PlayerId, int DiceCount) : DomainEvent;