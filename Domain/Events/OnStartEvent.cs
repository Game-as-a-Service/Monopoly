using Domain.Common;

namespace Domain.Events;

public record OnStartEvent(string PlayerId, int GainMoney, decimal TotalMoney) : DomainEvent;