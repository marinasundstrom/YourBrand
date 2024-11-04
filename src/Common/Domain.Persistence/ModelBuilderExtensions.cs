using Microsoft.EntityFrameworkCore;

using YourBrand.Domain.Persistence.EntityConfigurations;

namespace YourBrand.Domain.Persistence;

public static class ModelBuilderExtensions
{
    public static ModelBuilder ApplyDomainEntityConfigurations(this ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(OutboxMessageConfiguration).Assembly);

        return modelBuilder;
    }
}