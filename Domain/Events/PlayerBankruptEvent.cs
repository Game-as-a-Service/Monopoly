using Domain.Common;

namespace Domain.Events;

public record PlayerBankruptEvent(string PlayerId) : DomainEvent;