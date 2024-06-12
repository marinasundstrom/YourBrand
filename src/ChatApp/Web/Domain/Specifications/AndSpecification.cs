using System.Linq.Expressions;
using LinqKit;

namespace ChatApp.Domain.Specifications;

public class AndSpecification<T> : BaseSpecification<T>
{
    public AndSpecification(ISpecification<T> left, ISpecification<T> right)
    {
        var param = Expression.Parameter(typeof(T));

        Criteria = left.Criteria.And(right.Criteria).Expand();

        Includes.AddRange(left.Includes);
        Includes.AddRange(right.Includes);

        left.IncludeStrings
            .Where(i => !IncludeStrings.Contains(i))
            .ForEach(i => IncludeStrings.Add(i));

        right.IncludeStrings
            .Where(i => !IncludeStrings.Contains(i))
            .ForEach(i => IncludeStrings.Add(i));
    }
}
