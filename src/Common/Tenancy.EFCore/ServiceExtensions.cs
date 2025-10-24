using System;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace YourBrand.Tenancy;

public static class ServiceExtensions
{
    public static IServiceCollection AddTenancyInterceptor(this IServiceCollection services)
    {
        services.TryAddScoped<SetTenantSaveChangesInterceptor>();
        return services;
    }

    public static IServiceCollection AddTenantDatabasePerTenant(this IServiceCollection services, Action<TenantDatabasePerTenantBuilder> configure)
    {
        ArgumentNullException.ThrowIfNull(configure);

        services.AddOptions<TenantDatabasePerTenantOptions>();

        var builder = new TenantDatabasePerTenantBuilder(services);
        configure(builder);

        return services;
    }
}