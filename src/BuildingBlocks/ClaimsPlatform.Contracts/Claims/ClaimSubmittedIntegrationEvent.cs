namespace ClaimsPlatform.Contracts.Claims;

public sealed record ClaimSubmittedIntegrationEvent(
    Guid EventId,
    Guid ClaimId,
    string ClaimNumber,
    string ClaimType,
    string ClaimantName,
    string ClaimantEmail,
    decimal EstimatedLoss,
    DateTimeOffset OccurredAt);