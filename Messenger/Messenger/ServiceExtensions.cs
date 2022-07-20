using YourBrand.Identity;
using YourBrand.Messenger.Application.Common.Interfaces;

namespace YourBrand.Messenger;

public static class ServiceExtensions
{
    public static IServiceCollection AddServices(this IServiceCollection services)
    {
        services.AddIdentityServices();

        return services;
    }
}