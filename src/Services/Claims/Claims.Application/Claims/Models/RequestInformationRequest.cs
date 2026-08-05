namespace Claims.Application.Claims.Models;

public sealed record RequestInformationRequest(
    Guid OfficerId,
    string Question);