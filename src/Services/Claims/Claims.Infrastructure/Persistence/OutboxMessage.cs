namespace Claims.Infrastructure.Persistence;

public sealed class OutboxMessage
{
    private OutboxMessage()
    {
    }

    public OutboxMessage(
        Guid id,
        string eventType,
        Guid aggregateId,
        string payload,
        DateTimeOffset occurredAt)
    {
        if (id == Guid.Empty)
        {
            throw new ArgumentException(
                "Outbox message ID cannot be empty.",
                nameof(id));
        }

        if (aggregateId == Guid.Empty)
        {
            throw new ArgumentException(
                "Aggregate ID cannot be empty.",
                nameof(aggregateId));
        }

        if (string.IsNullOrWhiteSpace(eventType))
        {
            throw new ArgumentException(
                "Event type is required.",
                nameof(eventType));
        }

        if (string.IsNullOrWhiteSpace(payload))
        {
            throw new ArgumentException(
                "Event payload is required.",
                nameof(payload));
        }

        Id = id;
        EventType = eventType.Trim();
        AggregateId = aggregateId;
        Payload = payload;
        OccurredAt = occurredAt;
    }

    public Guid Id { get; private set; }

    public string EventType { get; private set; } = string.Empty;

    public Guid AggregateId { get; private set; }

    public string Payload { get; private set; } = string.Empty;

    public DateTimeOffset OccurredAt { get; private set; }

    public DateTimeOffset? ProcessedAt { get; private set; }

    public string? Error { get; private set; }

    public int RetryCount { get; private set; }

    public void MarkProcessed(DateTimeOffset processedAt)
    {
        ProcessedAt = processedAt;
        Error = null;
    }

    public void MarkFailed(string error)
    {
        RetryCount++;
        Error = string.IsNullOrWhiteSpace(error)
            ? "Unknown publishing error."
            : error.Trim();
    }
}