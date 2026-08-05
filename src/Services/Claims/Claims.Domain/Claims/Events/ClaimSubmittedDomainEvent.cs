using Claims.Domain.Common;

namespace Claims.Domain.Claims.Events;

public sealed record ClaimSubmittedDomainEvent(
    Guid ClaimId,
    string ClaimNumber,
    ClaimType ClaimType,
    string ClaimantName,
    string ClaimantEmail,
    decimal EstimatedLoss) : DomainEvent;