using System.Text.Json;
using Claims.Application.Abstractions.Persistence;
using Claims.Domain.Claims;
using Claims.Domain.Claims.Events;
using Claims.Domain.Common;
using ClaimsPlatform.Contracts.Claims;
using Microsoft.EntityFrameworkCore;

namespace Claims.Infrastructure.Persistence;

public sealed class ClaimsDbContext :
    DbContext,
    IUnitOfWork
{
    private static readonly JsonSerializerOptions JsonOptions =
        new(JsonSerializerDefaults.Web);

    public ClaimsDbContext(
        DbContextOptions<ClaimsDbContext> options)
        : base(options)
    {
    }

    public DbSet<Claim> Claims => Set<Claim>();

    public DbSet<ClaimInformationRequest> InformationRequests =>
        Set<ClaimInformationRequest>();

    public DbSet<OutboxMessage> OutboxMessages =>
        Set<OutboxMessage>();

    public override async Task<int> SaveChangesAsync(
        CancellationToken cancellationToken = default)
    {
        AddOutboxMessages();

        int result =
            await base.SaveChangesAsync(cancellationToken);

        ClearDomainEvents();

        return result;
    }

    protected override void OnModelCreating(
        ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(
            typeof(ClaimsDbContext).Assembly);
    }

    private void AddOutboxMessages()
    {
        List<Entity> entities =
            ChangeTracker
                .Entries<Entity>()
                .Where(entry => entry.Entity.DomainEvents.Count > 0)
                .Select(entry => entry.Entity)
                .ToList();

        List<IDomainEvent> domainEvents =
            entities
                .SelectMany(entity => entity.DomainEvents)
                .ToList();

        foreach (IDomainEvent domainEvent in domainEvents)
        {
            OutboxMessage? outboxMessage =
                MapToOutboxMessage(domainEvent);

            if (outboxMessage is not null)
            {
                OutboxMessages.Add(outboxMessage);
            }
        }
    }

    private static OutboxMessage? MapToOutboxMessage(
        IDomainEvent domainEvent)
    {
        return domainEvent switch
        {
            ClaimSubmittedDomainEvent submittedEvent =>
                CreateClaimSubmittedMessage(submittedEvent),

            ClaimStatusChangedDomainEvent statusEvent =>
                CreateClaimStatusChangedMessage(statusEvent),

            _ => null
        };
    }

    private static OutboxMessage CreateClaimSubmittedMessage(
        ClaimSubmittedDomainEvent domainEvent)
    {
        var integrationEvent =
            new ClaimSubmittedIntegrationEvent(
                domainEvent.EventId,
                domainEvent.ClaimId,
                domainEvent.ClaimNumber,
                domainEvent.ClaimType.ToString(),
                domainEvent.ClaimantName,
                domainEvent.ClaimantEmail,
                domainEvent.EstimatedLoss,
                domainEvent.OccurredAt);

        return new OutboxMessage(
            integrationEvent.EventId,
            nameof(ClaimSubmittedIntegrationEvent),
            integrationEvent.ClaimId,
            JsonSerializer.Serialize(
                integrationEvent,
                JsonOptions),
            integrationEvent.OccurredAt);
    }

    private static OutboxMessage CreateClaimStatusChangedMessage(
        ClaimStatusChangedDomainEvent domainEvent)
    {
        var integrationEvent =
            new ClaimStatusChangedIntegrationEvent(
                domainEvent.EventId,
                domainEvent.ClaimId,
                domainEvent.ClaimNumber,
                domainEvent.PreviousStatus.ToString(),
                domainEvent.NewStatus.ToString(),
                domainEvent.EstimatedLoss,
                domainEvent.ApprovedSettlementAmount,
                domainEvent.OccurredAt);

        return new OutboxMessage(
            integrationEvent.EventId,
            nameof(ClaimStatusChangedIntegrationEvent),
            integrationEvent.ClaimId,
            JsonSerializer.Serialize(
                integrationEvent,
                JsonOptions),
            integrationEvent.OccurredAt);
    }

    private void ClearDomainEvents()
    {
        ChangeTracker
            .Entries<Entity>()
            .Select(entry => entry.Entity)
            .ToList()
            .ForEach(entity => entity.ClearDomainEvents());
    }
}