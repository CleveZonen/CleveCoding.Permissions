#region

using System.Linq.Expressions;

#endregion

namespace CleveCoding.Kernel.Extensions;

public static class IQueryableExtensions
{
	/// <summary>
	///     Apply the <paramref name="filters" /> to a IQueryable-collection using <see cref="System.Linq.Expressions" />.
	/// </summary>
	/// <typeparam name="TModel"></typeparam>
	/// <param name="source"></param>
	/// <param name="filters"></param>
	/// <returns></returns>
	public static IQueryable<TModel> ApplyBaseFilters<TModel>(this IQueryable<TModel> source, IEnumerable<IPropertyFilter> filters)
	{
		var propertyFilters = filters as IPropertyFilter[] ?? [.. filters];
		return filters is null || propertyFilters.Length == 0 || !source.Any()
			? source // nothing to apply
			: propertyFilters.Aggregate(source, (query, filter) => query.ApplyFilter(filter));
	}

	private static IQueryable<TModel> ApplyFilter<TModel>(this IQueryable<TModel> query, IPropertyFilter filter)
	{
		// build the filter expression for native types.
		return filter switch
		{
			IPropertyFilter<TModel, string?> { HasValue: true } f => query.AddWhere(f, x => x.BuildContainsExpression()),
			IPropertyFilter<TModel, char?> { HasValue: true } f => query.AddWhere(f, x => x.BuildEqualExpression()),
			IPropertyFilter<TModel, bool?> { HasValue: true } f => query.AddWhere(f, x => x.BuildEqualExpression()),
			IPropertyFilter<TModel, int?> { HasValue: true } f => query.AddWhere(f, x => x.BuildEqualExpression()),
			IPropertyFilter<TModel, long?> { HasValue: true } f => query.AddWhere(f, x => x.BuildEqualExpression()),
			IPropertyFilter<TModel, short?> { HasValue: true } f => query.AddWhere(f, x => x.BuildEqualExpression()),
			IPropertyFilter<TModel, decimal?> { HasValue: true } f => query.AddWhere(f, x => x.BuildEqualExpression()),
			IPropertyFilter<TModel, double?> { HasValue: true } f => query.AddWhere(f, x => x.BuildEqualExpression()),
			IPropertyFilter<TModel, float?> { HasValue: true } f => query.AddWhere(f, x => x.BuildEqualExpression()),
			IPropertyFilter<TModel, DateTime?> { HasValue: true } f => query.AddWhere(f, x => x.BuildEqualExpression()),
			_ => query
		};
	}

	/// <summary>
	///     Add an Expression based on the filter to filter the source.
	/// </summary>
	/// <param name="source"></param>
	/// <param name="filter"></param>
	/// <param name="buildExpression"></param>
	/// <typeparam name="TModel"></typeparam>
	/// <typeparam name="TValue"></typeparam>
	/// <returns></returns>
	public static IQueryable<TModel> AddWhere<TModel, TValue>(this IQueryable<TModel> source, IPropertyFilter<TModel, TValue> filter,
		Func<IPropertyFilter<TModel, TValue>, Expression> buildExpression)
	{
		// build the filter expression and apply it to the source.
		return source.Where(Expression.Lambda<Func<TModel, bool>>(buildExpression(filter), filter.Property.Parameters));
	}
}