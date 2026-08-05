using Claims.Domain.Claims;

namespace Claims.Application.Abstractions.Persistence;

public interface IClaimRepository
{
    Task AddAsync(
        Claim claim,
        CancellationToken cancellationToken = default);

    Task<Claim?> GetByIdAsync(
        Guid claimId,
        CancellationToken cancellationToken = default);

    Task<Claim?> GetByClaimNumberAsync(
        string claimNumber,
        CancellationToken cancellationToken = default);

    Task<bool> ClaimNumberExistsAsync(
        string claimNumber,
        CancellationToken cancellationToken = default);
}