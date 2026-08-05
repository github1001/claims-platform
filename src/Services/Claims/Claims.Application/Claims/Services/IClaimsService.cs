using Claims.Application.Claims.Models;

namespace Claims.Application.Claims.Services;

public interface IClaimsService
{
    Task<SubmitClaimResponse> SubmitAsync(
        SubmitClaimRequest request,
        CancellationToken cancellationToken = default);

    Task<ClaimResponse?> GetByIdAsync(
        Guid claimId,
        CancellationToken cancellationToken = default);

    Task<ClaimResponse?> GetByClaimNumberAsync(
        string claimNumber,
        CancellationToken cancellationToken = default);

    Task<ClaimResponse?> StartReviewAsync(
        Guid claimId,
        StartReviewRequest request,
        CancellationToken cancellationToken = default);

    Task<ClaimResponse?> RequestInformationAsync(
        Guid claimId,
        RequestInformationRequest request,
        CancellationToken cancellationToken = default);

    Task<ClaimResponse?> ProvideInformationAsync(
        Guid claimId,
        Guid informationRequestId,
        ProvideInformationRequest request,
        CancellationToken cancellationToken = default);

    Task<ClaimResponse?> ApproveAsync(
        Guid claimId,
        ApproveClaimRequest request,
        CancellationToken cancellationToken = default);

    Task<ClaimResponse?> RejectAsync(
        Guid claimId,
        RejectClaimRequest request,
        CancellationToken cancellationToken = default);

    Task<ClaimResponse?> SettleAsync(
        Guid claimId,
        SettleClaimRequest request,
        CancellationToken cancellationToken = default);
}