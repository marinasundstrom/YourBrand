using System;
using System.Linq.Expressions;
using System.Reflection;

namespace Worker.Application;

public static class ExpressionTreesExtension
{
    static readonly MethodInfo orderByMethodInfo;
    static readonly MethodInfo orderByDescendingMethodInfo;
    static readonly MethodInfo lambdaMethodInfo;

    static ExpressionTreesExtension()
    {
        var queryableType = typeof(Queryable);

        orderByMethodInfo = queryableType.GetMethods(BindingFlags.Public | BindingFlags.Static).First(mi =>
        {
            return mi.Name == "OrderBy" && mi.GetParameters().First().ParameterType.Name.Contains("IQueryable");
        })!;

        orderByDescendingMethodInfo = queryableType.GetMethods(BindingFlags.Public | BindingFlags.Static).First(mi =>
        {
            return mi.Name == "OrderByDescending" && mi.GetParameters().First().ParameterType.Name.Contains("IQueryable");
        })!;

        var expressionType = typeof(Expression);

        lambdaMethodInfo = expressionType.GetMethods(BindingFlags.Public | BindingFlags.Static).First(mi =>
        {
            return mi.Name == "Lambda";
        })!;
    }

    public static IQueryable<T> OrderBy<T>(this IQueryable<T> source, string propertyName, SortDirection sortDirection)
    {
        if (propertyName is null) return source;

        if (sortDirection == SortDirection.None) return source;

        var type = typeof(T);

        /*
        var propInfo = type.GetProperty(propertyName);

        if(propInfo is null)
        {
            throw new PropertyNotFoundException(type, propertyName);
        }
        */

        var _orderByMethodInfo = sortDirection switch
        {
            SortDirection.Ascending => orderByMethodInfo,
            SortDirection.Descending => orderByDescendingMethodInfo,
            _ => throw new Exception()
        };

        LambdaExpression lambdaExpression = CreateOrderByExpression(type, propertyName)!;

        var methodInfo = _orderByMethodInfo.MakeGenericMethod(type, lambdaExpression.Body.Type);

        return (IQueryable<T>)methodInfo.Invoke(null, new object[] { source, lambdaExpression })!;
    }


    private static LambdaExpression CreateOrderByExpression(Type type, string p)
    {
        var parameterExpression = Expression.Parameter(type, "x");
        //var propertyAccess = Expression.MakeMemberAccess(parameterExpression, propInfo);

        var propertyAccess = MakeAccessExpression(parameterExpression, p);

        var method = lambdaMethodInfo
            .MakeGenericMethod(typeof(Func<,>)
            .MakeGenericType(type, propertyAccess.Type));

        return (LambdaExpression)method.Invoke(null, new object[] { propertyAccess, new ParameterExpression[] { parameterExpression } })!;
    }

    static MemberExpression MakeAccessExpression(ParameterExpression parameterExpression, string p)
    {
        var parts = p.Split('.');

        MemberExpression? memberExpression = null;

        foreach (var part in parts)
        {
            if (memberExpression is null)
            {
                memberExpression = Expression.Property(parameterExpression, part);
            }
            else
            {
                memberExpression = Expression.Property(memberExpression, part);
            }
        }

        return memberExpression!;
    }
}

public enum SortDirection
{
    None,
    Ascending,
    Descending
}

public class PropertyNotFoundException : Exception
{
    public PropertyNotFoundException(Type type, string propertyName)
        : base($"Type {type.FullName} does not have a property named {propertyName}.")
    {
    }
}