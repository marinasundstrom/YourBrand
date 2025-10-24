using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace YourBrand.Tenancy;

public static class DbContextOptionsBuilderExtensions
{
    public static DbContextOptionsBuilder UseTenancyInterceptor(this DbContextOptionsBuilder options, IServiceProvider serviceProvider)
    {
        options.AddInterceptors(
            serviceProvider.GetRequiredService<SetTenantSaveChangesInterceptor>());

        return options;
    }

    public static DbContextOptionsBuilder UseTenantDatabasePerTenant<TContext>(this DbContextOptionsBuilder options, IServiceProvider serviceProvider)
        where TContext : DbContext
    {
        var interceptor = serviceProvider.GetService<TenantDbConnectionInterceptor<TContext>>();

        if (interceptor is not null)
        {
            options.AddInterceptors(interceptor);
        }

        return options;
    }
}