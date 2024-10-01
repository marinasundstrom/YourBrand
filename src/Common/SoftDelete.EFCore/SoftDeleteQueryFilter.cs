using System.Linq.Expressions;
using System.Reflection;

namespace YourBrand.Domain;

public static class SoftDeleteQueryFilter
{
    static readonly Type softDeleteInterface = typeof(ISoftDeletable);
    static readonly PropertyInfo deletedProperty = softDeleteInterface.GetProperty(nameof(ISoftDeletable.Deleted));
    private static Expression<Func<ISoftDeletable, bool>> expression;

    public static Expression<Func<ISoftDeletable, bool>>? GetFilter()
    {
        if (expression is null)
        {
            var param = Expression.Parameter(softDeleteInterface, "entity");
            var body = Expression.Equal(Expression.Property(param, deletedProperty!), Expression.Constant(null));
            expression = Expression.Lambda<Func<ISoftDeletable, bool>>(body, param);
        }

        return expression;
    }

    public static bool CanApplyTo(Type entityType) => softDeleteInterface.IsAssignableFrom(entityType);
}