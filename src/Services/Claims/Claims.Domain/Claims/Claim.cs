using Claims.Domain.Claims.Events;
using Claims.Domain.Common;
using Claims.Domain.Exceptions;

namespace Claims.Domain.Claims;

public sealed class Claim : Entity
{
    private readonly List<ClaimInformationRequest> _informationRequests = [];
    

    private Claim()
    {
    }
    private Claim(
        Guid id,
        string claimNumber,
        ClaimType claimType,
        string claimantName,
        string claimantEmail,
        DateTime incidentDate,
        string incidentDescription,
        decimal estimatedLoss,
        DateTimeOffset createdAt)
        : base(id)
    {
        ClaimNumber = claimNumber;
        ClaimType = claimType;
        ClaimantName = claimantName;
        ClaimantEmail = claimantEmail;
        IncidentDate = incidentDate;
        IncidentDescription = incidentDescription;
        EstimatedLoss = estimatedLoss;
        Status = ClaimStatus.Submitted;
        CreatedAt = createdAt;
        UpdatedAt = createdAt;
    }

    public string ClaimNumber { get; private set; } = string.Empty;

    public ClaimType ClaimType { get; private set; }

    public string ClaimantName { get; private set; } = string.Empty;

    public string ClaimantEmail { get; private set; } = string.Empty;

    public DateTime IncidentDate { get; private set; }

    public string IncidentDescription { get; private set; } = string.Empty;

    public decimal EstimatedLoss { get; private set; }

    public decimal? ApprovedSettlementAmount { get; private set; }

    public ClaimStatus Status { get; private set; }

    public Guid? AssignedOfficerId { get; private set; }

    public string? DecisionReason { get; private set; }

    public DateTimeOffset CreatedAt { get; private set; }

    public DateTimeOffset UpdatedAt { get; private set; }

    public DateTimeOffset? ClosedAt { get; private set; }

    public uint Version { get; private set; }

    public IReadOnlyCollection<ClaimInformationRequest> InformationRequests =>
        _informationRequests.AsReadOnly();

    public static Claim Submit(
        Guid id,
        string claimNumber,
        ClaimType claimType,
        string claimantName,
        string claimantEmail,
        DateTime incidentDate,
        string incidentDescription,
        decimal estimatedLoss,
        DateTimeOffset createdAt)
    {
        ValidateSubmission(
            claimNumber,
            claimantName,
            claimantEmail,
            incidentDate,
            incidentDescription,
            estimatedLoss,
            createdAt);

        var claim = new Claim(
            id,
            claimNumber.Trim(),
            claimType,
            claimantName.Trim(),
            claimantEmail.Trim(),
            incidentDate,
            incidentDescription.Trim(),
            estimatedLoss,
            createdAt);

        claim.RaiseDomainEvent(
            new ClaimSubmittedDomainEvent(
                claim.Id,
                claim.ClaimNumber,
                claim.ClaimType,
                claim.ClaimantName,
                claim.ClaimantEmail,
                claim.EstimatedLoss));

        return claim;
    }

    public void StartReview(
        Guid officerId,
        DateTimeOffset changedAt)
    {
        if (officerId == Guid.Empty)
        {
            throw new DomainException("Claims officer ID is required.");
        }

        EnsureStatus(
            ClaimStatus.Submitted,
            "Only submitted claims can enter review.");

        AssignedOfficerId = officerId;

        ChangeStatus(
            ClaimStatus.UnderReview,
            changedAt,
            reason: null);
    }

    public Guid RequestAdditionalInformation(
        string question,
        Guid officerId,
        DateTimeOffset requestedAt)
    {
        EnsureStatus(
            ClaimStatus.UnderReview,
            "Additional information can only be requested while a claim is under review.");

        if (AssignedOfficerId != officerId)
        {
            throw new DomainException(
                "Only the assigned claims officer can request additional information.");
        }

        bool alreadyHasPendingRequest =
            _informationRequests.Any(
                request =>
                    request.Status == InformationRequestStatus.Pending);

        if (alreadyHasPendingRequest)
        {
            throw new DomainException(
                "The claim already has a pending information request.");
        }

        var request = new ClaimInformationRequest(
            Guid.NewGuid(),
            Id,
            question,
            officerId,
            requestedAt);

        _informationRequests.Add(request);

        ChangeStatus(
            ClaimStatus.AdditionalInformationRequired,
            requestedAt,
            question.Trim());

        return request.Id;
    }

    public void ProvideAdditionalInformation(
        Guid informationRequestId,
        string response,
        DateTimeOffset respondedAt)
    {
        EnsureStatus(
            ClaimStatus.AdditionalInformationRequired,
            "The claim is not awaiting additional information.");

        ClaimInformationRequest request =
            _informationRequests.SingleOrDefault(
                request => request.Id == informationRequestId)
            ?? throw new DomainException(
                "The information request was not found.");

        request.Respond(response, respondedAt);

        ChangeStatus(
            ClaimStatus.UnderReview,
            respondedAt,
            "Requested information supplied by claimant.");
    }

    public void Approve(
        decimal settlementAmount,
        string reason,
        Guid officerId,
        DateTimeOffset approvedAt)
    {
        EnsureAssignedOfficer(officerId);

        EnsureStatus(
            ClaimStatus.UnderReview,
            "Only claims under review can be approved.");

        if (settlementAmount <= 0)
        {
            throw new DomainException(
                "The settlement amount must be greater than zero.");
        }

        if (string.IsNullOrWhiteSpace(reason))
        {
            throw new DomainException(
                "An approval reason is required.");
        }

        ApprovedSettlementAmount = settlementAmount;
        DecisionReason = reason.Trim();

        ChangeStatus(
            ClaimStatus.Approved,
            approvedAt,
            DecisionReason);
    }

    public void Reject(
        string reason,
        Guid officerId,
        DateTimeOffset rejectedAt)
    {
        EnsureAssignedOfficer(officerId);

        EnsureStatus(
            ClaimStatus.UnderReview,
            "Only claims under review can be rejected.");

        if (string.IsNullOrWhiteSpace(reason))
        {
            throw new DomainException(
                "A rejection reason is required.");
        }

        ApprovedSettlementAmount = null;
        DecisionReason = reason.Trim();
        ClosedAt = rejectedAt;

        ChangeStatus(
            ClaimStatus.Rejected,
            rejectedAt,
            DecisionReason);
    }

    public void Settle(
        Guid officerId,
        DateTimeOffset settledAt)
    {
        EnsureAssignedOfficer(officerId);

        EnsureStatus(
            ClaimStatus.Approved,
            "Only approved claims can be settled.");

        if (ApprovedSettlementAmount is null)
        {
            throw new DomainException(
                "An approved settlement amount is required.");
        }

        ClosedAt = settledAt;

        ChangeStatus(
            ClaimStatus.Settled,
            settledAt,
            "Approved settlement completed.");
    }

    private void ChangeStatus(
        ClaimStatus newStatus,
        DateTimeOffset changedAt,
        string? reason)
    {
        ClaimStatus previousStatus = Status;

        Status = newStatus;
        UpdatedAt = changedAt;
        Version++;

        RaiseDomainEvent(
            new ClaimStatusChangedDomainEvent(
                Id,
                ClaimNumber,
                previousStatus,
                newStatus,
                EstimatedLoss,
                ApprovedSettlementAmount,
                reason));
    }

    private void EnsureStatus(
        ClaimStatus expectedStatus,
        string errorMessage)
    {
        if (Status != expectedStatus)
        {
            throw new DomainException(errorMessage);
        }
    }

    private void EnsureAssignedOfficer(Guid officerId)
    {
        if (officerId == Guid.Empty)
        {
            throw new DomainException("Claims officer ID is required.");
        }

        if (AssignedOfficerId != officerId)
        {
            throw new DomainException(
                "Only the assigned claims officer can perform this action.");
        }
    }

    private static void ValidateSubmission(
        string claimNumber,
        string claimantName,
        string claimantEmail,
        DateTime incidentDate,
        string incidentDescription,
        decimal estimatedLoss,
        DateTimeOffset createdAt)
    {
        if (string.IsNullOrWhiteSpace(claimNumber))
        {
            throw new DomainException("Claim number is required.");
        }

        if (string.IsNullOrWhiteSpace(claimantName))
        {
            throw new DomainException("Claimant name is required.");
        }

        if (string.IsNullOrWhiteSpace(claimantEmail))
        {
            throw new DomainException("Claimant email is required.");
        }

        if (!claimantEmail.Contains('@'))
        {
            throw new DomainException(
                "A valid claimant email address is required.");
        }

        if (incidentDate.Date > createdAt.UtcDateTime.Date)
        {
            throw new DomainException(
                "Incident date cannot be in the future.");
        }

        if (string.IsNullOrWhiteSpace(incidentDescription))
        {
            throw new DomainException(
                "Incident description is required.");
        }

        if (estimatedLoss <= 0)
        {
            throw new DomainException(
                "Estimated loss must be greater than zero.");
        }
    }
}