using Claims.Application.Abstractions;
using Claims.Application.Abstractions.Persistence;
using Claims.Infrastructure.Persistence;
using Claims.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Claims.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddClaimsInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        string connectionString =
            configuration.GetConnectionString("ClaimsDatabase")
            ?? throw new InvalidOperationException(
                "Connection string 'ClaimsDatabase' was not found.");

        services.AddDbContext<ClaimsDbContext>(
            options =>
                options.UseSqlServer(
                    connectionString,
                    sqlServerOptions =>
                    {
                        sqlServerOptions.EnableRetryOnFailure(
                            maxRetryCount: 5,
                            maxRetryDelay: TimeSpan.FromSeconds(10),
                            errorNumbersToAdd: null);
                    }));

        services.AddScoped<IClaimRepository, ClaimRepository>();

        services.AddScoped<IUnitOfWork>(
            serviceProvider =>
                serviceProvider.GetRequiredService<ClaimsDbContext>());
        services.AddSingleton<IClock, SystemClock>();

        return services;
    }
}