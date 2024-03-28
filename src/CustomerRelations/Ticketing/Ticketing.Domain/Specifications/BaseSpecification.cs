using System.Linq;
using System.Linq.Expressions;
using LinqKit;

namespace YourBrand.Ticketing.Domain.Specifications;

public abstract class BaseSpecification<T> : ISpecification<T>
{
    public Expression<Func<T, bool>> Criteria { get; set; } = null!;

    public List<Expression<Func<T, object>>> Includes { get; } = new List<Expression<Func<T, object>>>();

    public List<string> IncludeStrings { get; } = new List<string>();

    protected virtual void AddInclude(Expression<Func<T, object>> includeExpression)
    {
        Includes.Add(includeExpression);
    }

    protected virtual void AddInclude(string includeString)
    {
        IncludeStrings.Add(includeString);
    }
}

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

public class OrSpecification<T> : BaseSpecification<T>
{
    public OrSpecification(ISpecification<T> left, ISpecification<T> right)
    {
        var param = Expression.Parameter(typeof(T));

        Criteria = left.Criteria.Or(right.Criteria).Expand();

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

public static class SpecificationExtensions
{
    public static ISpecification<T> And<T>(this ISpecification<T> left, ISpecification<T> right)
    {
        return new AndSpecification<T>(left, right);
    }

    public static ISpecification<T> Or<T>(this ISpecification<T> left, ISpecification<T> right)
    {
        return new OrSpecification<T>(left, right);
    }
}