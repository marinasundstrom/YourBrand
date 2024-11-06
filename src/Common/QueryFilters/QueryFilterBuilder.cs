using System.Collections;
using System.Linq.Expressions;

using LinqKit;

namespace YourBrand;

public interface IQueryFilter
{
    bool CanApplyTo(Type entityType);
    Expression ApplyTo(ParameterExpression parameter);
}

public interface IQueryFilterCollection : IEnumerable<IQueryFilter>
{
    /// <summary>
    /// Add query filter to builder.
    /// </summary>
    /// <param name="expression"></param>
    /// <returns>This QueryFilterBuilder.</returns>
    IQueryFilterCollection Add(IQueryFilter queryFilter);
}

public class QueryFilterCollection : IQueryFilterCollection
{
    private readonly List<IQueryFilter> _items = new List<IQueryFilter>();

    public IQueryFilterCollection Add(IQueryFilter queryFilter)
    {
        _items.Add(queryFilter);

        return this;
    }

    public IEnumerator<IQueryFilter> GetEnumerator()
    {
        return _items.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}

public interface IQueryFilterBuilder : IQueryFilterCollection
{
    public Type EntityType { get; }

    /// <summary>
    /// Builds a LambdaExpression from combining the query filters.
    /// </summary>
    /// <returns>If there are query filters, a lambda expression, otherwise null.</returns>
    LambdaExpression? Build();
}

public sealed class QueryFilterBuilder : IQueryFilterBuilder
{
    readonly List<IQueryFilter> _queryFilters = new List<IQueryFilter>();

    public QueryFilterBuilder(Type entityType)
    {
        EntityType = entityType;
    }
    public Type EntityType { get; }

    /// <summary>
    /// Builds a LambdaExpression from combining the query filters.
    /// </summary>
    /// <returns>If there are query filters, a lambda expression, otherwise null.</returns>
    public LambdaExpression? Build()
    {
        var parameter = Expression.Parameter(EntityType, "entity");

        Expression? queryFilterExpression = null;

        if (_queryFilters.Count == 0)
        {
            return null;
        }

        foreach (var queryFilter in _queryFilters)
        {
            queryFilterExpression = queryFilterExpression is null
                ? queryFilter.ApplyTo(parameter)
                : Expression.AndAlso(
                    queryFilterExpression,
                    queryFilter.ApplyTo(parameter))
                    .Expand();
        }

        return Expression.Lambda(queryFilterExpression.Expand(), parameter);
    }

    /// <summary>
    /// Add query filter to builder.
    /// </summary>
    /// <param name="queryFilter"></param>
    /// <returns>This QueryFilterBuilder.</returns>
    public IQueryFilterBuilder Add(IQueryFilter queryFilter)
    {
        _queryFilters.Add(queryFilter);
        return this;
    }

    IQueryFilterCollection IQueryFilterCollection.Add(IQueryFilter queryFilter)
    {
        Add(queryFilter);
        return this;
    }

    public IEnumerator<IQueryFilter> GetEnumerator()
    {
        return _queryFilters.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}