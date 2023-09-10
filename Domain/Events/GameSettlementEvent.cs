using Domain.Common;

namespace Domain.Events;

public record GameSettlementEvent(int Rounds, params Player[] Players) : DomainEvent;
