using Domain.Common;

namespace Domain.Events;

public record PlayerCanBuildHouseEvent(string PlayerId, string LandId, int HouseCount, decimal UpgradePrice) : DomainEvent;