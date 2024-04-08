using YourBrand.HumanResources.Application.Common.Interfaces;
using YourBrand.HumanResources.Services;
using YourBrand.Identity;

namespace YourBrand.HumanResources;

public static class ServiceExtensions
{
    public static IServiceCollection AddServices(this IServiceCollection services)
    {
        services.AddUserContext();

        services.AddScoped<IEventPublisher, EventPublisher>();

        return services;
    }
}