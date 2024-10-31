using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

using YourBrand.Domain.Persistence.Interceptors;

namespace YourBrand.Domain.Persistence;

public static class ServiceExtensions
{
    public static IServiceCollection AddDomainPersistence<TContext>(this IServiceCollection services, IConfiguration configuration)
        where TContext : DbContext
    {
        services.TryAddScoped<OutboxSaveChangesInterceptor>();

        return services;
    }
}