using Domain.Common;

namespace Domain.Events;

public record PlayerNeedToChooseDirectionEvent(string GameId, string PlayerId, string[] Directions) : DomainEvent(GameId);
