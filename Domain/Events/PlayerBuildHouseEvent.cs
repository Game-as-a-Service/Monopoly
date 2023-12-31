using Domain.Common;

namespace Domain.Events;

public record PlayerBuildHouseEvent(string PlayerId, string LandId, decimal PlayerMoney, int HouseCount)
    : DomainEvent;

public record PlayerCannotBuildHouseEvent(string PlayerId, string LandId)
    : DomainEvent;

public record PlayerTooPoorToBuildHouseEvent(string PlayerId, string BlockId, decimal PlayerMoney, decimal UpgradePrice)
    : DomainEvent;