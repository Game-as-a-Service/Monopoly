namespace Domain.Common;

public abstract class AbstractAggregateRoot
{
    private readonly List<DomainEvent> domainEvents;

    public IReadOnlyList<DomainEvent> DomainEvents => domainEvents.AsReadOnly();

    protected AbstractAggregateRoot()
    {
        domainEvents = new List<DomainEvent>();
    }

    public void AddDomainEvent(DomainEvent domainEvent)
    {
        if (domainEvent == null || domainEvent == DomainEvent.EmptyEvent)
        {
            return;
        }
        domainEvents.Add(domainEvent);
    }

    public void AddDomainEvent(List<DomainEvent> domainEvents)
    {
        this.domainEvents.AddRange(domainEvents.TakeWhile(x => x != DomainEvent.EmptyEvent));
    }

    public void ClearEvent()
    {
        domainEvents.Clear();
    }
}