namespace Domain.Common;

public record DomainEvent(string GameId)
{
    private static readonly DomainEvent emptyEvent = new(string.Empty);
    public static DomainEvent EmptyEvent => emptyEvent;
}
public record EmptyEvent() : DomainEvent(string.Empty);
