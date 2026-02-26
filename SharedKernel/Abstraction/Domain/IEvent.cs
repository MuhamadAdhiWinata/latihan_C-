using MediatR;

namespace SharedKernel.Abstraction.Domain;

public interface IEvent : INotification
{
    Guid EventId => Guid.CreateVersion7();
    public DateTime OccurredOn => DateTime.Now;
    public string EventType => GetType().AssemblyQualifiedName ?? throw new InvalidOperationException();
}

public interface IDomainEvent : IEvent
{
}