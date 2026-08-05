namespace Claims.Application.Claims.Models;

public sealed record ApproveClaimRequest(
    Guid OfficerId,
    decimal SettlementAmount,
    string Reason);