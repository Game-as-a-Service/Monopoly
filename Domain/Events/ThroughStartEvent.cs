using Domain.Common;

namespace Domain.Events;

public record ThroughStartEvent(string PlayerId, decimal GainMoney, decimal TotalMoney) : DomainEvent;