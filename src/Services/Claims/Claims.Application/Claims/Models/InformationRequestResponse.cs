namespace Claims.Application.Claims.Models;

public sealed record InformationRequestResponse(
    Guid Id,
    string Question,
    string? Response,
    Guid RequestedByOfficerId,
    DateTimeOffset RequestedAt,
    DateTimeOffset? RespondedAt,
    string Status);