using Domain.Common;

namespace Domain.Events;

public record PlayerBuildHouseEvent(string PlayerId, string BlockId, decimal PlayerMoney, int House)
    : DomainEvent;

public record PlayerCannotBuildHouseEvent(string PlayerId, string BlockId)
    : DomainEvent;

public record PlayerTooPoorToBuildHouseEvent(string PlayerId, string BlockId, decimal PlayerMoney, decimal UpgradePrice)
    : DomainEvent;