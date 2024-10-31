using Microsoft.EntityFrameworkCore;

using YourBrand.Domain;

namespace YourBrand.Sales.Persistence;

public static class ModelBuilderExtensions
{
    public static ModelBuilder ConfigQueryFilterForModel(this ModelBuilder modelBuilder, ITenantContext tenantContext)
    {
        foreach (var type in modelBuilder.Model
            .GetEntityTypes())
        {
            var clrType = type.ClrType;

            if (type.IsOwned())
            {
                Console.WriteLine($"Skipping type {clrType} because it is defined as owned.");
                continue;
            }

            if (!clrType.IsAssignableTo(typeof(IEntity)))
            {
                Console.WriteLine($"Skipping type {clrType} because it is not implementing IEntity.");
                continue;
            }

            var entityTypeBuilder = modelBuilder.Entity(clrType);

            entityTypeBuilder
                .AddTenantIndex()
                .AddOrganizationIndex();

            try
            {
                entityTypeBuilder.RegisterQueryFilters(builder =>
                {
                    builder.AddTenancyFilter(tenantContext);
                    builder.AddSoftDeleteFilter();
                });
            }
            catch (InvalidOperationException exc)
                when (exc.MatchQueryFilterExceptions(clrType))
            { }
        }

        return modelBuilder;
    }
}