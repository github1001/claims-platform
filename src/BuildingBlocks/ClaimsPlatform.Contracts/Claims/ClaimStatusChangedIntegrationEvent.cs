namespace ClaimsPlatform.Contracts.Claims;

public sealed record ClaimStatusChangedIntegrationEvent(
    Guid EventId,
    Guid ClaimId,
    string ClaimNumber,
    string PreviousStatus,
    string NewStatus,
    decimal EstimatedLoss,
    decimal? ApprovedSettlementAmount,
    DateTimeOffset OccurredAt);