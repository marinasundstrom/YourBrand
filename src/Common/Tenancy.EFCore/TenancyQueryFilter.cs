using System.Linq.Expressions;
using System.Reflection;

using LinqKit;

namespace YourBrand.Tenancy;

public class TenancyQueryFilter : IQueryFilter
{
    static readonly Type hasTenantInterface = typeof(IHasTenant);
    static readonly PropertyInfo tenantIdProperty = hasTenantInterface.GetProperty(nameof(IHasTenant.TenantId));

    static readonly Type nullableTenantIdType = typeof(Nullable<TenantId>);
    static readonly MethodInfo getValueOrDefaultMethod = nullableTenantIdType.GetMethods().First(x => x.Name == "GetValueOrDefault");

    private readonly Expression expression;

    public TenancyQueryFilter(Expression<Func<TenantId?>> tenantIdAccessor, bool allowNull = false)
    {
        var param = Expression.Parameter(hasTenantInterface, "entity");

        Expression body = Expression.Equal(
            Expression.Property(param, tenantIdProperty!),
            Expression.Call(Expression.Invoke(tenantIdAccessor), getValueOrDefaultMethod));

        if (allowNull)
        {
            var body2 = Expression.Equal(Expression.Invoke(tenantIdAccessor), Expression.Constant(null, typeof(TenantId?)));
            body = Expression.OrElse(body2, body).Expand();
        }

        expression = Expression.Lambda<Func<IHasTenant, bool>>(body.Expand(), param);
    }

    public bool CanApplyTo(Type entityType) => hasTenantInterface.IsAssignableFrom(entityType);

    public Expression ApplyTo(ParameterExpression parameter)
    {
        return Expression.Invoke(expression, Expression.Convert(parameter, typeof(IHasTenant)));
    }
}
