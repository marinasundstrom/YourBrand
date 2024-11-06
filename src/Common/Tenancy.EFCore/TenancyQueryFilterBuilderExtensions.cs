using System.Linq.Expressions;

namespace YourBrand.Tenancy;

public static class TenancyQueryFilterBuilderExtensions
{
    /// <summary>
    /// Adds query filter for filtering by tenancy. If applicable to the current entity type.
    /// </summary>
    /// <param name="queryFilterBuilder"></param>
    /// <param name="tenantIdAccessor"></param>
    /// <returns></returns>
    public static IQueryFilterCollection AddTenancyFilter(this IQueryFilterBuilder queryFilterBuilder,
        Expression<Func<TenantId?>> tenantIdAccessor)
    {
        var queryFilter = new TenancyQueryFilter(tenantIdAccessor);

        if (!queryFilter.CanApplyTo(queryFilterBuilder.EntityType))
            return queryFilterBuilder;

        queryFilterBuilder.Add(queryFilter);

        return queryFilterBuilder;
    }
}