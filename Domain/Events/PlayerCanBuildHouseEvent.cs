using Domain.Common;

namespace Domain.Events;

public record PlayerCanBuildHouseEvent(string GameId, string PlayerId, string BlockId, int HouseCount, decimal UpgradeMoney)
    : DomainEvent(GameId);