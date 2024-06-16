using YourBrand.ChatApp.Infrastructure.Persistence;

namespace YourBrand.ChatApp.Web.Extensions;

public static class HealthChecksExtensions
{
    public static IServiceCollection AddHealthChecksServices(this IServiceCollection services)
    {
        services
            .AddHealthChecks()
            .AddDbContextCheck<ApplicationDbContext>();

        return services;
    }
}