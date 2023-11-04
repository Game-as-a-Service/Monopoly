using Domain.Common;

namespace Domain.Events;

public record GameStartEvent(string GameStage, string CurrentPlayer)
    : DomainEvent;

public record OnlyOnePersonEvent(string GameStage)
    : DomainEvent;

public record SomePlayersPreparingEvent(string GameStage, params string[] Players)
    : DomainEvent;