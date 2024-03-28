using YourBrand.ApiKeys.Application.Common.Interfaces;
using YourBrand.Identity;

namespace YourBrand.ApiKeys;

public static class ServiceExtensions
{
    public static IServiceCollection AddServices(this IServiceCollection services)
    {
        services.AddIdentityServices();

        return services;
    }
}