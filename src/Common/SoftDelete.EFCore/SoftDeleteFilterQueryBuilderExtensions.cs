using System.Linq.Expressions;

namespace YourBrand.Domain;

public static class SoftDeleteFilterQueryBuilderExtensions
{
    /// <summary>
    /// Adds query filter for filtering by soft-deleted entities. If applicable to the current entity type.
    /// </summary>
    /// <param name="queryFilterBuilder"></param>
    /// <returns></returns>
    public static QueryFilterBuilder AddSoftDeleteFilter(this QueryFilterBuilder queryFilterBuilder)
    {
        if (SoftDeleteQueryFilter.CanApplyTo(queryFilterBuilder.EntityType))
        {
            var softDeleteFilter = SoftDeleteQueryFilter.GetFilter();

            queryFilterBuilder.AddFilter(
                Expression.Invoke(softDeleteFilter, Expression.Convert(queryFilterBuilder.Parameter, typeof(ISoftDeletable))));
        }

        return queryFilterBuilder;
    }
}