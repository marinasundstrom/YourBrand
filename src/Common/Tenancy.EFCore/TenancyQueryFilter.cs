using System.Linq.Expressions;
using System.Reflection;
using LinqKit;

namespace YourBrand.Tenancy;

public static class TenancyQueryFilter
{
    static readonly Type hasTenantInterface = typeof(IHasTenant);
    static readonly PropertyInfo tenantIdProperty = hasTenantInterface.GetProperty(nameof(IHasTenant.TenantId));

    static readonly Type nullableTenantIdType = typeof(Nullable<TenantId>);
    static readonly MethodInfo getValueOrDefaultMethod = nullableTenantIdType.GetMethods().First(x => x.Name == "GetValueOrDefault");

    public static Expression<Func<IHasTenant, bool>> GetFilter(
        Expression<Func<TenantId?>> tenantIdAccessor, bool allowNull = false)
    {
        var param = Expression.Parameter(hasTenantInterface, "entity");

        Expression body = Expression.Equal(
            Expression.Property(param, tenantIdProperty!),
            //Expression.Invoke(tenantIdAccessor));
            Expression.Call(Expression.Invoke(tenantIdAccessor), getValueOrDefaultMethod));

        if (allowNull) 
        {
            var body2 = Expression.Equal(Expression.Invoke(tenantIdAccessor), Expression.Constant(null)); //new TenantId()
            body = Expression.OrElse(body2, body);
            var body2 = Expression.Equal(Expression.Invoke(tenantIdAccessor), Expression.Constant(null, typeof(TenantId?))); //new TenantId()
        }

        return Expression.Lambda<Func<IHasTenant, bool>>(body, param);
    }

    public static bool CanApplyTo(Type entityType) => hasTenantInterface.IsAssignableFrom(entityType);
}