using System.Linq.Expressions;

using LinqKit;

namespace YourBrand;

public class QueryFilterBuilder
{
    List<Expression> _queryFilters = new List<Expression>();

    /// <summary>
    /// Constructs a QueryFilterBuilder.
    /// </summary>
    /// <param name="entityType">The type of the parameter.</param>
    public QueryFilterBuilder(Type entityType)
    {
        Parameter = Expression.Parameter(entityType, "entity");
        EntityType = entityType;
    }

    /// <summary>
    /// Gets the entity type
    /// </summary>
    /// <value></value>
    public Type EntityType { get; private set; }

    /// <summary>
    /// Gets the parameter used for the LambdaExpression.
    /// </summary>
    /// <value>A LambdaExpression</value>
    public ParameterExpression Parameter { get; private set; }

    /// <summary>
    /// Builds a LambdaExpression from combining the query filters.
    /// </summary>
    /// <returns>If there are query filters, a lambda expression, otherwise null.</returns>
    public LambdaExpression? Build()
    {
        Expression? queryFilter = null;

        if (_queryFilters.Count == 0)
        {
            return null;
        }

        foreach (var qf in _queryFilters)
        {
            queryFilter = queryFilter is null
                ? qf
                : Expression.AndAlso(
                    queryFilter,
                    qf)
                    .Expand();
        }

        return Expression.Lambda(queryFilter.Expand(), Parameter);
    }

    /// <summary>
    /// Add query filter to builder.
    /// </summary>
    /// <param name="expression"></param>
    /// <returns>This QueryFilterBuilder.</returns>
    public QueryFilterBuilder AddFilter(Expression expression)
    {
        _queryFilters.Add(expression);
        return this;
    }
}
