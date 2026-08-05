namespace Claims.Application.Claims.Models;

public sealed record ClaimResponse(
    Guid Id,
    string ClaimNumber,
    string ClaimType,
    string ClaimantName,
    string ClaimantEmail,
    DateTime IncidentDate,
    string IncidentDescription,
    decimal EstimatedLoss,
    decimal? ApprovedSettlementAmount,
    string Status,
    Guid? AssignedOfficerId,
    string? DecisionReason,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt,
    DateTimeOffset? ClosedAt,
    uint Version,
    IReadOnlyCollection<InformationRequestResponse> InformationRequests);