using Domain.Common;

namespace Domain.Events;

public record OnStartEvent(string GameId, string PlayerId, int GainMoney, decimal TotalMoney) : DomainEvent(GameId);