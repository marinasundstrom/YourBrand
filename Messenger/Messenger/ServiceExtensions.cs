using YourBrand.Messenger.Application.Common.Interfaces;
using YourBrand.Messenger.Services;

namespace YourBrand.Messenger;

public static class ServiceExtensions
{
    public static IServiceCollection AddServices(this IServiceCollection services)
    {
        services.AddScoped<ICurrentUserService, CurrentUserService>();

        return services;
    }
}