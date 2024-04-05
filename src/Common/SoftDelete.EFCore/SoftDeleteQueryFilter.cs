using System.Linq.Expressions;
using System.Reflection;

namespace YourBrand.Domain;

public static class SoftDeleteQueryFilter
{
    static readonly Type softDeleteInterface = typeof(ISoftDelete);
    static readonly PropertyInfo deletedProperty = softDeleteInterface.GetProperty(nameof(ISoftDelete.Deleted));
    private static Expression<Func<ISoftDelete, bool>> expression;

    public static Expression<Func<ISoftDelete, bool>>? GetFilter()
    {
        if (expression is null)
        {
            var param = Expression.Parameter(softDeleteInterface, "entity");
            var body = Expression.Equal(Expression.Property(param, deletedProperty!), Expression.Constant(null));
            expression = Expression.Lambda<Func<ISoftDelete, bool>>(body, param);
        }
        
        return expression;
    }

    public static bool CanApplyTo(Type entityType) => softDeleteInterface.IsAssignableFrom(entityType);
}