using DomainClaim = Claims.Domain.Claims.Claim;

using Claims.Domain.Claims;
using Claims.Domain.Claims.Events;
using Claims.Domain.Exceptions;

namespace Claims.UnitTests;

public sealed class ClaimTests
{
    private static readonly DateTimeOffset Now =
        new(2026, 8, 5, 9, 0, 0, TimeSpan.Zero);

    private static readonly Guid OfficerId =
        Guid.Parse("11111111-1111-1111-1111-111111111111");

    [Fact]
    public void Submit_WithValidDetails_CreatesSubmittedClaim()
    {
        DomainClaim claim = CreateClaim();

        Assert.Equal(ClaimStatus.Submitted, claim.Status);
        Assert.Equal("CLM-2026-000001", claim.ClaimNumber);
        Assert.Equal(ClaimType.Motor, claim.ClaimType);
        Assert.Equal(25_000m, claim.EstimatedLoss);
        Assert.Null(claim.AssignedOfficerId);
        Assert.Null(claim.ApprovedSettlementAmount);

        Assert.Contains(
            claim.DomainEvents,
            domainEvent => domainEvent is ClaimSubmittedDomainEvent);
    }

    [Fact]
    public void Submit_WithFutureIncidentDate_ThrowsDomainException()
    {
        Action action = () =>
            Claim.Submit(
                Guid.NewGuid(),
                "CLM-2026-000001",
                ClaimType.Motor,
                "Alex Tan",
                "alex@example.com",
                Now.AddDays(1).UtcDateTime,
                "Vehicle collision",
                25_000m,
                Now);

        DomainException exception =
            Assert.Throws<DomainException>(action);

        Assert.Equal(
            "Incident date cannot be in the future.",
            exception.Message);
    }

    [Fact]
    public void StartReview_FromSubmitted_AssignsOfficerAndChangesStatus()
    {
        DomainClaim claim = CreateClaim();

        claim.StartReview(OfficerId, Now.AddMinutes(5));

        Assert.Equal(ClaimStatus.UnderReview, claim.Status);
        Assert.Equal(OfficerId, claim.AssignedOfficerId);
        Assert.Equal(1u, claim.Version);

        ClaimStatusChangedDomainEvent domainEvent =
            Assert.IsType<ClaimStatusChangedDomainEvent>(
                claim.DomainEvents.Last());

        Assert.Equal(
            ClaimStatus.Submitted,
            domainEvent.PreviousStatus);

        Assert.Equal(
            ClaimStatus.UnderReview,
            domainEvent.NewStatus);
    }

    [Fact]
    public void StartReview_WhenAlreadyUnderReview_ThrowsDomainException()
    {
        DomainClaim claim = CreateClaim();

        claim.StartReview(
            OfficerId,
            Now.AddMinutes(5));

        Action action = () =>
            claim.StartReview(
                OfficerId,
                Now.AddMinutes(10));

        DomainException exception =
            Assert.Throws<DomainException>(action);

        Assert.Equal(
            "Only submitted claims can enter review.",
            exception.Message);
    }

    [Fact]
    public void RequestAdditionalInformation_CreatesPendingRequest()
    {
        DomainClaim claim = CreateClaim();

        claim.StartReview(
            OfficerId,
            Now.AddMinutes(5));

        Guid requestId =
            claim.RequestAdditionalInformation(
                "Please provide the police report.",
                OfficerId,
                Now.AddMinutes(10));

        ClaimInformationRequest request =
            Assert.Single(claim.InformationRequests);

        Assert.Equal(requestId, request.Id);

        Assert.Equal(
            InformationRequestStatus.Pending,
            request.Status);

        Assert.Equal(
            ClaimStatus.AdditionalInformationRequired,
            claim.Status);
    }

    [Fact]
    public void RequestAdditionalInformation_ByDifferentOfficer_Throws()
    {
        DomainClaim claim = CreateClaim();

        claim.StartReview(
            OfficerId,
            Now.AddMinutes(5));

        Action action = () =>
            claim.RequestAdditionalInformation(
                "Please provide supporting evidence.",
                Guid.NewGuid(),
                Now.AddMinutes(10));

        DomainException exception =
            Assert.Throws<DomainException>(action);

        Assert.Equal(
            "Only the assigned claims officer can request additional information.",
            exception.Message);
    }

    [Fact]
    public void ProvideAdditionalInformation_ReturnsClaimToUnderReview()
    {
        DomainClaim claim = CreateClaim();

        claim.StartReview(
            OfficerId,
            Now.AddMinutes(5));

        Guid requestId =
            claim.RequestAdditionalInformation(
                "Please provide the repair quotation.",
                OfficerId,
                Now.AddMinutes(10));

        claim.ProvideAdditionalInformation(
            requestId,
            "The repair quotation is RM 18,500.",
            Now.AddMinutes(20));

        Assert.Equal(
            ClaimStatus.UnderReview,
            claim.Status);

        ClaimInformationRequest request =
            Assert.Single(claim.InformationRequests);

        Assert.Equal(
            InformationRequestStatus.Responded,
            request.Status);

        Assert.Equal(
            "The repair quotation is RM 18,500.",
            request.Response);
    }

    [Fact]
    public void Approve_WhenUnderReview_StoresSettlementDecision()
    {
        DomainClaim claim = CreateClaim();

        claim.StartReview(
            OfficerId,
            Now.AddMinutes(5));

        claim.Approve(
            18_500m,
            "Damage is covered by the policy.",
            OfficerId,
            Now.AddMinutes(15));

        Assert.Equal(
            ClaimStatus.Approved,
            claim.Status);

        Assert.Equal(
            18_500m,
            claim.ApprovedSettlementAmount);

        Assert.Equal(
            "Damage is covered by the policy.",
            claim.DecisionReason);
    }

    [Fact]
    public void Approve_ByUnassignedOfficer_Throws()
    {
        DomainClaim claim = CreateClaim();

        claim.StartReview(
            OfficerId,
            Now.AddMinutes(5));

        Action action = () =>
            claim.Approve(
                18_500m,
                "Damage is covered.",
                Guid.NewGuid(),
                Now.AddMinutes(15));

        Assert.Throws<DomainException>(action);
    }

    [Fact]
    public void Reject_WhenUnderReview_ClosesClaim()
    {
        DomainClaim claim = CreateClaim();

        claim.StartReview(
            OfficerId,
            Now.AddMinutes(5));

        DateTimeOffset rejectedAt =
            Now.AddMinutes(15);

        claim.Reject(
            "The incident is excluded by the policy.",
            OfficerId,
            rejectedAt);

        Assert.Equal(
            ClaimStatus.Rejected,
            claim.Status);

        Assert.Equal(
            rejectedAt,
            claim.ClosedAt);

        Assert.Null(
            claim.ApprovedSettlementAmount);
    }

    [Fact]
    public void Settle_WhenApproved_ClosesClaim()
    {
        DomainClaim claim = CreateClaim();

        claim.StartReview(
            OfficerId,
            Now.AddMinutes(5));

        claim.Approve(
            18_500m,
            "Damage is covered.",
            OfficerId,
            Now.AddMinutes(15));

        DateTimeOffset settledAt =
            Now.AddMinutes(30);

        claim.Settle(
            OfficerId,
            settledAt);

        Assert.Equal(
            ClaimStatus.Settled,
            claim.Status);

        Assert.Equal(
            settledAt,
            claim.ClosedAt);

        Assert.Equal(
            18_500m,
            claim.ApprovedSettlementAmount);
    }

    [Fact]
    public void Settle_WhenNotApproved_Throws()
    {
        DomainClaim claim = CreateClaim();

        claim.StartReview(
            OfficerId,
            Now.AddMinutes(5));

        Action action = () =>
            claim.Settle(
                OfficerId,
                Now.AddMinutes(30));

        DomainException exception =
            Assert.Throws<DomainException>(action);

        Assert.Equal(
            "Only approved claims can be settled.",
            exception.Message);
    }

    private static DomainClaim CreateClaim()
    {
        return DomainClaim.Submit(
            Guid.NewGuid(),
            "CLM-2026-000001",
            ClaimType.Motor,
            "Alex Tan",
            "alex@example.com",
            Now.AddDays(-1).UtcDateTime,
            "Vehicle collision at a road junction.",
            25_000m,
            Now);
    }
}