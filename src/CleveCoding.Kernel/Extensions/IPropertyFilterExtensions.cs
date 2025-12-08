#region

using System.Linq.Expressions;
using System.Reflection;

#endregion

namespace CleveCoding.Kernel.Extensions;

public static class IPropertyFilterExtensions
{
    private static readonly MethodInfo? ContainsMethodInfo = typeof(string).GetMethod(nameof(string.Contains), [typeof(string)]);
    private static readonly MethodInfo? ToLowerMethodInfo = typeof(string).GetMethod(nameof(string.ToLower), Type.EmptyTypes);

    /// <summary>
    ///     Build Expression for string property to call Contains().
    /// </summary>
    /// <param name="filter"></param>
    /// <typeparam name="TModel"></typeparam>
    /// <returns></returns>
    public static MethodCallExpression BuildContainsExpression<TModel>(this IPropertyFilter<TModel, string?> filter)
    {
        // build the expression to call ToLower on the Property for insensitive-case search.
        // example: x => x.Property.ToLower()
        var toLowerExpression = Expression.Call(filter.Property.Body, ToLowerMethodInfo!);

        // build the expression to call the string.Contains() method.
        // example: x => x.Property.Contains(filter.Value)
        return Expression.Call(toLowerExpression, ContainsMethodInfo!, Expression.Constant(filter.Value?.ToLower()));
    }

    /// <summary>
    ///     Build Expression to compare using the equal operator '=='.
    /// </summary>
    /// <param name="filter"></param>
    /// <typeparam name="TModel"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    /// <returns></returns>
    public static BinaryExpression BuildEqualExpression<TModel, TValue>(this IPropertyFilter<TModel, TValue> filter)
    {
        var propertyType = filter.Property.Body.Type;

        // convert from Nullable to the Underlying Type.
        var propertyExpression =
            propertyType.IsGenericType && propertyType.GetGenericTypeDefinition() == typeof(Nullable<>)
                ? Expression.Convert(filter.Property.Body, Nullable.GetUnderlyingType(propertyType)!)
                : filter.Property.Body;

        // build the expression to compare with the equal operator '=='.
        // example: x => x.Property == filter.Value
        return Expression.Equal(propertyExpression, Expression.Constant(filter.Value));
    }
}