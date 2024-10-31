using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace YourBrand.Auditability;

public static class DbContextOptionsBuilderExtensions
{
    public static DbContextOptionsBuilder UseAuditabilityInterceptor(this DbContextOptionsBuilder options, IServiceProvider serviceProvider)
    {
        options.AddInterceptors(
            serviceProvider.GetRequiredService<AuditableEntitySaveChangesInterceptor>());

        return options;
    }
}