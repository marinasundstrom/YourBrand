using System.Linq.Expressions;
using System.Reflection;

namespace YourBrand.Tenancy;

public static class TenancyQueryFilter
{
    static readonly Type hasTenantInterface = typeof(IHasTenant);
    static readonly PropertyInfo tenantIdProperty = hasTenantInterface.GetProperty(nameof(IHasTenant.TenantId));

    public static Expression<Func<IHasTenant, bool>> GetFilter(Expression<Func<TenantId>> tenantIdAccessor)
    {
        var param = Expression.Parameter(hasTenantInterface, "entity");
        var body = Expression.Equal(Expression.Property(param, tenantIdProperty!), Expression.Invoke(tenantIdAccessor));
        return Expression.Lambda<Func<IHasTenant, bool>>(body, param);
    }

    public static bool CanApplyTo(Type entityType) => hasTenantInterface.IsAssignableFrom(entityType);
}