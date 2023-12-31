using Domain.Common;

namespace Domain.Events;

public record PlayerRedeemEvent(string PlayerId, decimal PlayerMoney, string LandId) : DomainEvent;

public record PlayerTooPoorToRedeemEvent(string PlayerId, decimal PlayerMoney, string BlockId, decimal RedeemPrice) : DomainEvent;

public record LandNotInMortgageEvent(string PlayerId, string BlockId) : DomainEvent;