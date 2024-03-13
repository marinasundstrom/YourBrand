using Microsoft.EntityFrameworkCore;
using YourBrand.Analytics.Domain.Specifications;

namespace YourBrand.Analytics.Infrastructure.Persistence;

public static class QuerySpecificationExtensions
{
    public static IQueryable<T> Where<T>(this IQueryable<T> query, ISpecification<T> spec) where T : class
    {
        // fetch a Queryable that includes all expression-based includes
        var queryableResultWithIncludes = spec.Includes
            .Aggregate(query,
                (current, include) => current.Include(include));

        // modify the IQueryable to include any string-based include statements
        var secondaryResult = spec.IncludeStrings
            .Aggregate(queryableResultWithIncludes,
                (current, include) => current.Include(include));

        // return the result of the query using the specification's criteria expression
        return secondaryResult.Where(spec.Criteria);
    }
}
