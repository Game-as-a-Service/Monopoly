using Domain.Common;

namespace Domain.Events;

public record PlayerRedeemEvent(string GameId, string PlayerId, decimal PlayerMoney, string BlockId)
    : DomainEvent(GameId);

public record PlayerTooPoorToRedeemEvent(string GameId, string PlayerId, decimal PlayerMoney, string BlockId, decimal RedeemPrice)
    : DomainEvent(GameId);

public record LandNotInMortgageEvent(string GameId, string PlayerId, string BlockId)
    : DomainEvent(GameId);