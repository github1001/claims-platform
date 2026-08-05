namespace Claims.Application.Claims.Models;

public sealed record RejectClaimRequest(
    Guid OfficerId,
    string Reason);