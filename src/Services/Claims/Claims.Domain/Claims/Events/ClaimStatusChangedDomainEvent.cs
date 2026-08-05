using Claims.Domain.Common;

namespace Claims.Domain.Claims.Events;

public sealed record ClaimStatusChangedDomainEvent(
    Guid ClaimId,
    string ClaimNumber,
    ClaimStatus PreviousStatus,
    ClaimStatus NewStatus,
    decimal EstimatedLoss,
    decimal? ApprovedSettlementAmount,
    string? Reason) : DomainEvent;