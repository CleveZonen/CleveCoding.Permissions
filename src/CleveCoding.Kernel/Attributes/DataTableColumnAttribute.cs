namespace CleveCoding.Kernel.Attributes;

/// <summary>
/// Markers for properties to get (primitive) data into data columns.
/// </summary>
[AttributeUsage(AttributeTargets.Property)]
public class DataTableColumnAttribute : Attribute;
