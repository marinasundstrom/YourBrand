using YourBrand.Identity;
using YourBrand.IdentityManagement.Application.Common.Interfaces;
using YourBrand.IdentityManagement.Services;

namespace YourBrand.IdentityManagement;

public static class ServiceExtensions
{
    public static IServiceCollection AddServices(this IServiceCollection services)
    {
        services.AddUserContext();

        services.AddScoped<IEventPublisher, EventPublisher>();

        return services;
    }
}