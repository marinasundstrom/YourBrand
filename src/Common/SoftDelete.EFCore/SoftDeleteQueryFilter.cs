using System.Linq.Expressions;
using System.Reflection;

using LinqKit;

namespace YourBrand.Domain;

public class SoftDeleteQueryFilter : IQueryFilter
{
    static readonly Type softDeleteInterface = typeof(ISoftDeletable);
    static readonly PropertyInfo deletedProperty = softDeleteInterface.GetProperty(nameof(ISoftDeletable.IsDeleted));

    private readonly Expression expression;

    public SoftDeleteQueryFilter()
    {
        var param = Expression.Parameter(softDeleteInterface, "entity");
        var body = Expression.Equal(Expression.Property(param, deletedProperty!), Expression.Constant(false));

        expression = Expression.Lambda<Func<ISoftDeletable, bool>>(body, param);
    }

    public bool CanApplyTo(Type entityType) => softDeleteInterface.IsAssignableFrom(entityType);

    public Expression ApplyTo(ParameterExpression parameter)
    {
        return Expression.Invoke(expression, Expression.Convert(parameter, typeof(ISoftDeletable)));
    }
}
