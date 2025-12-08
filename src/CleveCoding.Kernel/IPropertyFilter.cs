#region

using System.Linq.Expressions;

#endregion

namespace CleveCoding.Kernel;

public interface IPropertyFilter
{
    /// <summary>
    ///     Display name for the property to show on screen.
    /// </summary>
    string DisplayName { get; set; }

    /// <summary>
    ///     Indicator if the filter has a value.
    /// </summary>
    bool HasValue { get; }

    /// <summary>
    ///     Reset the Field to its original values.
    /// </summary>
    void Reset();
}

public interface IPropertyFilter<TValue> : IPropertyFilter
{
    /// <summary>
    ///     Value of the filter that should be applied.
    /// </summary>
    TValue? Value { get; set; }
}

public interface IPropertyFilter<TModel, TValue> : IPropertyFilter<TValue>
{
    /// <summary>
    ///     The property expression the filter should be applied to.
    /// </summary>
    Expression<Func<TModel, TValue>> Property { get; set; }
}