using Domain.Common;

namespace Domain.Events;

public record EndRoundEvent(string GameId, string PlayerId, string NextPlayerId)
    : DomainEvent(GameId);

public record EndRoundFailEvent(string GameId, string PlayerId)
    : DomainEvent(GameId);