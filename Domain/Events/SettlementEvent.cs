using Domain.Common;

namespace Domain.Events;

public record SettlementEvent(string PlayerId, int Rank)
    : DomainEvent;

public record BankruptEvent(string PlayerId)
    : DomainEvent;