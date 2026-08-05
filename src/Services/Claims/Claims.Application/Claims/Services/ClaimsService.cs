using Claims.Application.Abstractions;
using Claims.Application.Abstractions.Persistence;
using Claims.Application.Claims.Models;
using Claims.Domain.Claims;
using Claims.Domain.Exceptions;
using DomainClaim = Claims.Domain.Claims.Claim;

namespace Claims.Application.Claims.Services;

public sealed class ClaimsService : IClaimsService
{
    private readonly IClaimRepository _claimRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IClock _clock;

    public ClaimsService(
        IClaimRepository claimRepository,
        IUnitOfWork unitOfWork,
        IClock clock)
    {
        _claimRepository = claimRepository;
        _unitOfWork = unitOfWork;
        _clock = clock;
    }

    public async Task<SubmitClaimResponse> SubmitAsync(
        SubmitClaimRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        if (!Enum.TryParse(
                request.ClaimType,
                ignoreCase: true,
                out ClaimType claimType))
        {
            throw new DomainException(
                "Claim type must be either Motor or Property.");
        }

        DateTimeOffset submittedAt = _clock.UtcNow;

        string claimNumber =
            await GenerateClaimNumberAsync(
                submittedAt,
                cancellationToken);

        DomainClaim claim = DomainClaim.Submit(
            Guid.NewGuid(),
            claimNumber,
            claimType,
            request.ClaimantName,
            request.ClaimantEmail,
            request.IncidentDate,
            request.IncidentDescription,
            request.EstimatedLoss,
            submittedAt);

        await _claimRepository.AddAsync(
            claim,
            cancellationToken);

        await _unitOfWork.SaveChangesAsync(
            cancellationToken);

        return new SubmitClaimResponse(
            claim.Id,
            claim.ClaimNumber,
            claim.Status.ToString(),
            claim.CreatedAt);
    }

    public async Task<ClaimResponse?> GetByIdAsync(
        Guid claimId,
        CancellationToken cancellationToken = default)
    {
        DomainClaim? claim =
            await _claimRepository.GetByIdAsync(
                claimId,
                cancellationToken);

        return claim is null
            ? null
            : MapClaim(claim);
    }

    public async Task<ClaimResponse?> GetByClaimNumberAsync(
        string claimNumber,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(claimNumber))
        {
            return null;
        }

        DomainClaim? claim =
            await _claimRepository.GetByClaimNumberAsync(
                claimNumber.Trim(),
                cancellationToken);

        return claim is null
            ? null
            : MapClaim(claim);
    }

    public async Task<ClaimResponse?> StartReviewAsync(
        Guid claimId,
        StartReviewRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        DomainClaim? claim =
            await _claimRepository.GetByIdAsync(
                claimId,
                cancellationToken);

        if (claim is null)
        {
            return null;
        }

        claim.StartReview(
            request.OfficerId,
            _clock.UtcNow);

        await _unitOfWork.SaveChangesAsync(
            cancellationToken);

        return MapClaim(claim);
    }

    public async Task<ClaimResponse?> RequestInformationAsync(
        Guid claimId,
        RequestInformationRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        DomainClaim? claim =
            await _claimRepository.GetByIdAsync(
                claimId,
                cancellationToken);

        if (claim is null)
        {
            return null;
        }

        claim.RequestAdditionalInformation(
            request.Question,
            request.OfficerId,
            _clock.UtcNow);

        await _unitOfWork.SaveChangesAsync(
            cancellationToken);

        return MapClaim(claim);
    }

    public async Task<ClaimResponse?> ProvideInformationAsync(
        Guid claimId,
        Guid informationRequestId,
        ProvideInformationRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        DomainClaim? claim =
            await _claimRepository.GetByIdAsync(
                claimId,
                cancellationToken);

        if (claim is null)
        {
            return null;
        }

        claim.ProvideAdditionalInformation(
            informationRequestId,
            request.Response,
            _clock.UtcNow);

        await _unitOfWork.SaveChangesAsync(
            cancellationToken);

        return MapClaim(claim);
    }

    public async Task<ClaimResponse?> ApproveAsync(
        Guid claimId,
        ApproveClaimRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        DomainClaim? claim =
            await _claimRepository.GetByIdAsync(
                claimId,
                cancellationToken);

        if (claim is null)
        {
            return null;
        }

        claim.Approve(
            request.SettlementAmount,
            request.Reason,
            request.OfficerId,
            _clock.UtcNow);

        await _unitOfWork.SaveChangesAsync(
            cancellationToken);

        return MapClaim(claim);
    }

    public async Task<ClaimResponse?> RejectAsync(
        Guid claimId,
        RejectClaimRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        DomainClaim? claim =
            await _claimRepository.GetByIdAsync(
                claimId,
                cancellationToken);

        if (claim is null)
        {
            return null;
        }

        claim.Reject(
            request.Reason,
            request.OfficerId,
            _clock.UtcNow);

        await _unitOfWork.SaveChangesAsync(
            cancellationToken);

        return MapClaim(claim);
    }

    public async Task<ClaimResponse?> SettleAsync(
        Guid claimId,
        SettleClaimRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        DomainClaim? claim =
            await _claimRepository.GetByIdAsync(
                claimId,
                cancellationToken);

        if (claim is null)
        {
            return null;
        }

        claim.Settle(
            request.OfficerId,
            _clock.UtcNow);

        await _unitOfWork.SaveChangesAsync(
            cancellationToken);

        return MapClaim(claim);
    }

    private async Task<string> GenerateClaimNumberAsync(
        DateTimeOffset submittedAt,
        CancellationToken cancellationToken)
    {
        for (int attempt = 0; attempt < 10; attempt++)
        {
            string suffix =
                Guid.NewGuid()
                    .ToString("N")[..8]
                    .ToUpperInvariant();

            string claimNumber =
                $"CLM-{submittedAt:yyyy}-{suffix}";

            bool exists =
                await _claimRepository.ClaimNumberExistsAsync(
                    claimNumber,
                    cancellationToken);

            if (!exists)
            {
                return claimNumber;
            }
        }

        throw new InvalidOperationException(
            "Unable to generate a unique claim number.");
    }

    private static ClaimResponse MapClaim(
        DomainClaim claim)
    {
        InformationRequestResponse[] informationRequests =
            claim.InformationRequests
                .OrderBy(request => request.RequestedAt)
                .Select(
                    request =>
                        new InformationRequestResponse(
                            request.Id,
                            request.Question,
                            request.Response,
                            request.RequestedByOfficerId,
                            request.RequestedAt,
                            request.RespondedAt,
                            request.Status.ToString()))
                .ToArray();

        return new ClaimResponse(
            claim.Id,
            claim.ClaimNumber,
            claim.ClaimType.ToString(),
            claim.ClaimantName,
            claim.ClaimantEmail,
            claim.IncidentDate,
            claim.IncidentDescription,
            claim.EstimatedLoss,
            claim.ApprovedSettlementAmount,
            claim.Status.ToString(),
            claim.AssignedOfficerId,
            claim.DecisionReason,
            claim.CreatedAt,
            claim.UpdatedAt,
            claim.ClosedAt,
            claim.Version,
            informationRequests);
    }
}