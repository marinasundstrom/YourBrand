using System;

using YourBrand.HumanResources.Application.Common.Interfaces;
using YourBrand.HumanResources.Services;

namespace YourBrand.HumanResources;

public static class ServiceExtensions
{
    public static IServiceCollection AddServices(this IServiceCollection services)
    {
        services.AddScoped<ICurrentPersonService, CurrentPersonService>();

        services.AddScoped<IEventPublisher, EventPublisher>();

        return services;
    }
}