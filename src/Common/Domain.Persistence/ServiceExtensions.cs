using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using YourBrand.Domain.Persistence.Interceptors;

namespace YourBrand.Domain.Persistence;

public static class ServiceExtensions
{
    public static IServiceCollection AddDomainPersistence<TContext>(this IServiceCollection services, IConfiguration configuration)
        where TContext : DomainDbContext
    {
        services.AddScoped<DomainDbContext>(sp => sp.GetRequiredService<TContext>());

        services.AddScoped<OutboxSaveChangesInterceptor>();

        return services;
    }
}