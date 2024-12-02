using System.Linq.Expressions;

using Microsoft.EntityFrameworkCore;

using YourBrand.Auditability;
using YourBrand.Domain;
using YourBrand.Tenancy;

namespace YourBrand;

public static class ModelBuilderExtensions
{
    public static ModelBuilder ConfigureDomainModel(this ModelBuilder modelBuilder, Action<DomainModelConfigurator> configurator)
    {
        var domainModelConfigurator = new DomainModelConfigurator();

        configurator(domainModelConfigurator);

        var configuration = domainModelConfigurator.Build();

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

            if (type.BaseType?.IsAbstract() ?? false)  
            {
                Console.WriteLine($"Skipping type {clrType} because it is a derived entity type.");
                continue;
            }

            var entityTypeBuilder = modelBuilder.Entity(clrType);

            entityTypeBuilder
                .AddTenantIndex()
                .AddOrganizationIndex()
                .AddSoftDeleteIndex();

            try
            {
                configuration.ConfigureEntityType(entityTypeBuilder);
            }
            catch (InvalidOperationException exc)
                when (exc.MatchQueryFilterExceptions(clrType))
            { }
        }

        return modelBuilder;
    }
}

public static class SoftDeleteFilterQueryBuilderExtensions
{
    /// <summary>
    /// Adds query filter for filtering by soft-deleted entities. If applicable to the current entity type.
    /// </summary>
    /// <param name="configurator"></param>
    /// <returns></returns>
    public static DomainModelConfigurator AddSoftDeleteFilter(this DomainModelConfigurator configurator)
    {
        var queryFilter = new SoftDeleteQueryFilter();

        configurator.QueryFilters.Add(new SoftDeleteQueryFilter());

        return configurator;
    }
}

public static class TenancyQueryFilterBuilderExtensions
{
    /// <summary>
    /// Adds query filter for filtering by tenancy. If applicable to the current entity type.
    /// </summary>
    /// <param name="configurator"></param>
    /// <param name="tenantIdAccessor"></param>
    /// <returns></returns>
    public static DomainModelConfigurator AddTenancyFilter(this DomainModelConfigurator configurator,
        Expression<Func<TenantId?>> tenantIdAccessor)
    {
        var queryFilter = new TenancyQueryFilter(tenantIdAccessor);

        configurator.QueryFilters.Add(queryFilter);

        return configurator;
    }
}
