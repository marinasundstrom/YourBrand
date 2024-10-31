using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

using YourBrand.Domain.Persistence.Interceptors;

namespace YourBrand.Domain.Persistence;

public static class DbContextOptionsBuilderExtensions
{
    public static DbContextOptionsBuilder UseDomainInterceptors(this DbContextOptionsBuilder options, IServiceProvider serviceProvider)
    {
        options.AddInterceptors(
            serviceProvider.GetRequiredService<OutboxSaveChangesInterceptor>());

        return options;
    }
}