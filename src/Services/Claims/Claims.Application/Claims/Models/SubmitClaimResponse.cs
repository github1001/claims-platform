namespace Claims.Application.Claims.Models;

public sealed record SubmitClaimResponse(
    Guid Id,
    string ClaimNumber,
    string Status,
    DateTimeOffset SubmittedAt);