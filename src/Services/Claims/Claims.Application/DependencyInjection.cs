using Claims.Application.Claims.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Claims.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddClaimsApplication(
        this IServiceCollection services)
    {
        services.AddScoped<IClaimsService, ClaimsService>();

        return services;
    }
}