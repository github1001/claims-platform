using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Claims.Infrastructure.Persistence.Configurations;

public sealed class OutboxMessageConfiguration :
    IEntityTypeConfiguration<OutboxMessage>
{
    public void Configure(
        EntityTypeBuilder<OutboxMessage> builder)
    {
        builder.ToTable(
            "outbox_messages",
            tableBuilder =>
                tableBuilder.HasCheckConstraint(
                    "CK_outbox_messages_payload_json",
                    "ISJSON([payload]) = 1"));

        builder.HasKey(message => message.Id);

        builder.Property(message => message.Id)
            .HasColumnName("id")
            .ValueGeneratedNever();

        builder.Property(message => message.EventType)
            .HasColumnName("event_type")
            .HasMaxLength(300)
            .IsRequired();

        builder.Property(message => message.AggregateId)
            .HasColumnName("aggregate_id")
            .IsRequired();

        builder.Property(message => message.Payload)
            .HasColumnName("payload")
            .HasColumnType("nvarchar(max)")
            .IsRequired();

        builder.Property(message => message.OccurredAt)
            .HasColumnName("occurred_at")
            .IsRequired();

        builder.Property(message => message.ProcessedAt)
            .HasColumnName("processed_at");

        builder.Property(message => message.Error)
            .HasColumnName("error")
            .HasMaxLength(4000);

        builder.Property(message => message.RetryCount)
            .HasColumnName("retry_count")
            .IsRequired();

        builder.HasIndex(message => message.ProcessedAt);

        builder.HasIndex(message => new
        {
            message.ProcessedAt,
            message.OccurredAt
        });
    }
}