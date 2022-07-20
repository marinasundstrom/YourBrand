using System;

using Microsoft.Extensions.DependencyInjection;

namespace YourBrand.Tenancy;

public static class ServicesExtensions
{
    public static IServiceCollection AddTenantService(this IServiceCollection services)
    {
        services.AddScoped<ITenantService, TenantService>();

        return services;
    }
}

