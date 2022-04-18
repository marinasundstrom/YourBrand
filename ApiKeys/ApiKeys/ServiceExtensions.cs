using YourBrand.ApiKeys.Application.Common.Interfaces;
using YourBrand.ApiKeys.Services;

namespace YourBrand.ApiKeys;

public static class ServiceExtensions
{
    public static IServiceCollection AddServices(this IServiceCollection services)
    {
        services.AddScoped<ICurrentUserService, CurrentUserService>();

        return services;
    }
}