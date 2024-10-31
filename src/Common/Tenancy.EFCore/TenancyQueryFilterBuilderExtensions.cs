using System.Linq.Expressions;

namespace YourBrand.Tenancy;

public static class TenancyQueryFilterBuilderExtensions
{
    /// <summary>
    /// Adds query filter for filtering by tenancy. If applicable to the current entity type.
    /// </summary>
    /// <param name="queryFilterBuilder"></param>
    /// <param name="tenantContext"></param>
    /// <returns></returns>
    public static QueryFilterBuilder AddTenancyFilter(this QueryFilterBuilder queryFilterBuilder, ITenantContext tenantContext)
    {
        if (TenancyQueryFilter.CanApplyTo(queryFilterBuilder.EntityType))
        {
            var tenantFilter = TenancyQueryFilter.GetFilter(() => tenantContext.TenantId);

            queryFilterBuilder.AddFilter(
                Expression.Invoke(tenantFilter, Expression.Convert(queryFilterBuilder.Parameter, typeof(IHasTenant))));
        }

        return queryFilterBuilder;
    }
}