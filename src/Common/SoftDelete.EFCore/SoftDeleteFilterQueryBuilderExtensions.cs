using System.Linq.Expressions;

namespace YourBrand.Domain;

public static class SoftDeleteFilterQueryBuilderExtensions
{
    /// <summary>
    /// Adds query filter for filtering by soft-deleted entities. If applicable to the current entity type.
    /// </summary>
    /// <param name="queryFilterBuilder"></param>
    /// <returns></returns>
    public static IQueryFilterCollection AddSoftDeleteFilter(this IQueryFilterBuilder queryFilterBuilder)
    {
        var queryFilter = new SoftDeleteQueryFilter();

        if (!queryFilter.CanApplyTo(queryFilterBuilder.EntityType))
            return queryFilterBuilder;

        queryFilterBuilder.Add(new SoftDeleteQueryFilter());

        return queryFilterBuilder;
    }
}
