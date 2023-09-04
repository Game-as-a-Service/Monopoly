namespace Domain.Common;

public record DomainEvent()
{
    private static readonly DomainEvent emptyEvent = new();
    public static DomainEvent EmptyEvent => emptyEvent;
}
public record EmptyEvent() : DomainEvent;
