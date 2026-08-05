using Claims.Domain.Common;
using Claims.Domain.Exceptions;

namespace Claims.Domain.Claims;

public sealed class ClaimInformationRequest : Entity
{
    private ClaimInformationRequest()
    {
    }

    internal ClaimInformationRequest(
        Guid id,
        Guid claimId,
        string question,
        Guid requestedByOfficerId,
        DateTimeOffset requestedAt)
        : base(id)
    {
        if (claimId == Guid.Empty)
        {
            throw new DomainException("Claim ID is required.");
        }

        if (requestedByOfficerId == Guid.Empty)
        {
            throw new DomainException(
                "The requesting claims officer ID is required.");
        }

        if (string.IsNullOrWhiteSpace(question))
        {
            throw new DomainException(
                "An information request question is required.");
        }

        ClaimId = claimId;
        Question = question.Trim();
        RequestedByOfficerId = requestedByOfficerId;
        RequestedAt = requestedAt;
        Status = InformationRequestStatus.Pending;
    }

    public Guid ClaimId { get; private set; }

    public string Question { get; private set; } = string.Empty;

    public string? Response { get; private set; }

    public Guid RequestedByOfficerId { get; private set; }

    public DateTimeOffset RequestedAt { get; private set; }

    public DateTimeOffset? RespondedAt { get; private set; }

    public InformationRequestStatus Status { get; private set; }

    internal void Respond(
        string response,
        DateTimeOffset respondedAt)
    {
        if (Status != InformationRequestStatus.Pending)
        {
            throw new DomainException(
                "Only pending information requests can be answered.");
        }

        if (string.IsNullOrWhiteSpace(response))
        {
            throw new DomainException(
                "The information response cannot be empty.");
        }

        Response = response.Trim();
        RespondedAt = respondedAt;
        Status = InformationRequestStatus.Responded;
    }

    internal void Cancel()
    {
        if (Status != InformationRequestStatus.Pending)
        {
            throw new DomainException(
                "Only pending information requests can be cancelled.");
        }

        Status = InformationRequestStatus.Cancelled;
    }
}