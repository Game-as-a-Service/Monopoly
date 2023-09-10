using Domain.Common;

namespace Domain.Events;

public record BankruptEvent(string PlayerId)
    : DomainEvent;