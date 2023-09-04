using Domain.Common;

namespace Domain.Events;

public record ThroughStartEvent(string PlayerId, int GainMoney, decimal TotalMoney) : DomainEvent;