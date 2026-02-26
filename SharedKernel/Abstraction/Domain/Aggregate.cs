namespace SharedKernel.Abstraction.Domain;

public class Aggregate
{
    private readonly List<IDomainEvent> _domainEvents = new();
    public IReadOnlyList<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    public IEvent[] ClearDomainEvents()
    {
        IEvent[] dequeuedEvents = _domainEvents.ToArray();

        _domainEvents.Clear();

        return dequeuedEvents;
    }

    public void AddDomainEvent(IDomainEvent domainEvent)
    {
        _domainEvents.Add(domainEvent);
    }
}