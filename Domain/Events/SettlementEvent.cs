using Domain.Common;

namespace Domain.Events;

public record SettlementEvent(string GameId, string PlayerId, int Rank)
    : DomainEvent(GameId);

public record BankruptEvent(string GameId, string PlayerId)
    : DomainEvent(GameId);