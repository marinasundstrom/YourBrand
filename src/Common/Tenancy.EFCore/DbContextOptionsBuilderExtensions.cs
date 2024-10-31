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
}