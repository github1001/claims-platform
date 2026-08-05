namespace Claims.Domain.Common;

public abstract record DomainEvent : IDomainEvent
{
    protected DomainEvent()
    {
        EventId = Guid.NewGuid();
        OccurredAt = DateTimeOffset.UtcNow;
    }

    public Guid EventId { get; }

    public DateTimeOffset OccurredAt { get; }
}