using Domain.Common;

namespace Domain.Events;

public record EndRoundEvent(string PlayerId, string NextPlayerId)
    : DomainEvent;

public record EndRoundFailEvent(string PlayerId)
    : DomainEvent;

public record SuspendRoundEvent(string PlayerId, int SuspendRounds)
    : DomainEvent;