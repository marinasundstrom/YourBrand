using System.Linq.Expressions;
using System.Reflection;

namespace YourBrand.Domain;

public static class SoftDeleteQueryFilter
{
    static readonly Type softDeleteInterface = typeof(ISoftDelete);
    static readonly PropertyInfo deletedProperty = softDeleteInterface.GetProperty(nameof(ISoftDelete.Deleted));

    public static Expression<Func<ISoftDelete, bool>>? GetFilter()
    {
        var param = Expression.Parameter(softDeleteInterface, "entity");
        var body = Expression.Equal(Expression.Property(param, deletedProperty!), Expression.Constant(null));
        return Expression.Lambda<Func<ISoftDelete, bool>>(body, param);
    }

    public static bool CanApplyTo(Type entityType) => softDeleteInterface.IsAssignableFrom(entityType);
}