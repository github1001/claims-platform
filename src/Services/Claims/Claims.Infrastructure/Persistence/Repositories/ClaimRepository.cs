using Claims.Application.Abstractions.Persistence;
using Claims.Domain.Claims;
using Microsoft.EntityFrameworkCore;

namespace Claims.Infrastructure.Persistence.Repositories;

public sealed class ClaimRepository : IClaimRepository
{
    private readonly ClaimsDbContext _dbContext;

    public ClaimRepository(ClaimsDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task AddAsync(
        Claim claim,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(claim);

        await _dbContext.Claims.AddAsync(
            claim,
            cancellationToken);
    }

    public Task<Claim?> GetByIdAsync(
        Guid claimId,
        CancellationToken cancellationToken = default)
    {
        return _dbContext.Claims
            .Include(claim => claim.InformationRequests)
            .SingleOrDefaultAsync(
                claim => claim.Id == claimId,
                cancellationToken);
    }

    public Task<Claim?> GetByClaimNumberAsync(
        string claimNumber,
        CancellationToken cancellationToken = default)
    {
        return _dbContext.Claims
            .Include(claim => claim.InformationRequests)
            .SingleOrDefaultAsync(
                claim => claim.ClaimNumber == claimNumber,
                cancellationToken);
    }

    public Task<bool> ClaimNumberExistsAsync(
        string claimNumber,
        CancellationToken cancellationToken = default)
    {
        return _dbContext.Claims.AnyAsync(
            claim => claim.ClaimNumber == claimNumber,
            cancellationToken);
    }
}