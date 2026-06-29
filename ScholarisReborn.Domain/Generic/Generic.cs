using System;
using System.Collections.Generic;
using System.Text;


public interface IDomainEvent
{
    DateTime OccurredAt { get; }
}

public abstract record DomainEvent : IDomainEvent
{
    public DateTime OccurredAt { get; } = DateTime.UtcNow;
}

public abstract class AggregateRoot
{
    private readonly List<IDomainEvent> _domainEvents = new();
    public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents;

    protected void RaiseEvent(IDomainEvent e) => _domainEvents.Add(e);
    public void ClearEvents() => _domainEvents.Clear();
}

public class DomainException : Exception
{
    public DomainException(string message) : base(message) { }
}

