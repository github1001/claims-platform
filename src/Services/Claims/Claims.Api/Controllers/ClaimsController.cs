using Claims.Application.Claims.Models;
using Claims.Application.Claims.Services;
using Microsoft.AspNetCore.Mvc;

namespace Claims.Api.Controllers;

[ApiController]
[Route("api/claims")]
public sealed class ClaimsController : ControllerBase
{
    private readonly IClaimsService _claimsService;

    public ClaimsController(
        IClaimsService claimsService)
    {
        _claimsService = claimsService;
    }

    [HttpPost]
    [ProducesResponseType<SubmitClaimResponse>(
        StatusCodes.Status201Created)]
    public async Task<ActionResult<SubmitClaimResponse>> Submit(
        [FromBody] SubmitClaimRequest request,
        CancellationToken cancellationToken)
    {
        SubmitClaimResponse response =
            await _claimsService.SubmitAsync(
                request,
                cancellationToken);

        return CreatedAtAction(
            nameof(GetById),
            new { claimId = response.Id },
            response);
    }

    [HttpGet("{claimId:guid}")]
    public async Task<ActionResult<ClaimResponse>> GetById(
        Guid claimId,
        CancellationToken cancellationToken)
    {
        ClaimResponse? claim =
            await _claimsService.GetByIdAsync(
                claimId,
                cancellationToken);

        return claim is null
            ? NotFound()
            : Ok(claim);
    }

    [HttpGet("reference/{claimNumber}")]
    public async Task<ActionResult<ClaimResponse>>
        GetByClaimNumber(
            string claimNumber,
            CancellationToken cancellationToken)
    {
        ClaimResponse? claim =
            await _claimsService.GetByClaimNumberAsync(
                claimNumber,
                cancellationToken);

        return claim is null
            ? NotFound()
            : Ok(claim);
    }

    [HttpPost("{claimId:guid}/start-review")]
    public async Task<ActionResult<ClaimResponse>> StartReview(
        Guid claimId,
        [FromBody] StartReviewRequest request,
        CancellationToken cancellationToken)
    {
        ClaimResponse? claim =
            await _claimsService.StartReviewAsync(
                claimId,
                request,
                cancellationToken);

        return claim is null
            ? NotFound()
            : Ok(claim);
    }

    [HttpPost("{claimId:guid}/request-information")]
    public async Task<ActionResult<ClaimResponse>>
        RequestInformation(
            Guid claimId,
            [FromBody] RequestInformationRequest request,
            CancellationToken cancellationToken)
    {
        ClaimResponse? claim =
            await _claimsService.RequestInformationAsync(
                claimId,
                request,
                cancellationToken);

        return claim is null
            ? NotFound()
            : Ok(claim);
    }

    [HttpPost(
        "{claimId:guid}/information-requests/{informationRequestId:guid}/response")]
    public async Task<ActionResult<ClaimResponse>>
        ProvideInformation(
            Guid claimId,
            Guid informationRequestId,
            [FromBody] ProvideInformationRequest request,
            CancellationToken cancellationToken)
    {
        ClaimResponse? claim =
            await _claimsService.ProvideInformationAsync(
                claimId,
                informationRequestId,
                request,
                cancellationToken);

        return claim is null
            ? NotFound()
            : Ok(claim);
    }

    [HttpPost("{claimId:guid}/approve")]
    public async Task<ActionResult<ClaimResponse>> Approve(
        Guid claimId,
        [FromBody] ApproveClaimRequest request,
        CancellationToken cancellationToken)
    {
        ClaimResponse? claim =
            await _claimsService.ApproveAsync(
                claimId,
                request,
                cancellationToken);

        return claim is null
            ? NotFound()
            : Ok(claim);
    }

    [HttpPost("{claimId:guid}/reject")]
    public async Task<ActionResult<ClaimResponse>> Reject(
        Guid claimId,
        [FromBody] RejectClaimRequest request,
        CancellationToken cancellationToken)
    {
        ClaimResponse? claim =
            await _claimsService.RejectAsync(
                claimId,
                request,
                cancellationToken);

        return claim is null
            ? NotFound()
            : Ok(claim);
    }

    [HttpPost("{claimId:guid}/settle")]
    public async Task<ActionResult<ClaimResponse>> Settle(
        Guid claimId,
        [FromBody] SettleClaimRequest request,
        CancellationToken cancellationToken)
    {
        ClaimResponse? claim =
            await _claimsService.SettleAsync(
                claimId,
                request,
                cancellationToken);

        return claim is null
            ? NotFound()
            : Ok(claim);
    }
}