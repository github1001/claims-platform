namespace Claims.Application.Claims.Models;

public sealed record SubmitClaimRequest(
    string ClaimType,
    string ClaimantName,
    string ClaimantEmail,
    DateTime IncidentDate,
    string IncidentDescription,
    decimal EstimatedLoss);