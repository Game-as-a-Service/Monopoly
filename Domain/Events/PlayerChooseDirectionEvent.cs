using Domain.Common;

namespace Domain.Events;

public record PlayerChooseDirectionEvent(string GameId, string PlayerId, string Direction): DomainEvent(GameId);
