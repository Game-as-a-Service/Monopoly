using Domain.Common;

namespace Domain.Events;

public record PlayerBuildHouseEvent(string GameId, string PlayerId, string BlockId, decimal PlayerMoney, int House)
    : DomainEvent(GameId);

public record PlayerCannotBuildHouseEvent(string GameId, string PlayerId, string BlockId)
    : DomainEvent(GameId);

public record PlayerTooPoorToBuildHouseEvent(string GameId, string PlayerId, string BlockId, decimal PlayerMoney, decimal UpgradePrice)
    : DomainEvent(GameId);