using Domain.Common;

namespace Domain.Events;

public record ThroughStartEvent(string GameId, string PlayerId, int GainMoney, decimal TotalMoney) : DomainEvent(GameId);