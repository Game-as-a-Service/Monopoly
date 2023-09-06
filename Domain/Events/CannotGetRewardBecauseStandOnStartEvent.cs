using Domain.Common;

namespace Domain.Events;

public record CannotGetRewardBecauseStandOnStartEvent(string PlayerId, decimal PlayerMoney) : DomainEvent;