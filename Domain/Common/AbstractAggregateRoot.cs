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
        domainEvents.Add(domainEvent);
    }

    public void AddDomainEvent(List<DomainEvent> domainEvents)
    {
        this.domainEvents.AddRange(domainEvents);
    }

    public void ClearEvent()
    {
        domainEvents.Clear();
    }
}
