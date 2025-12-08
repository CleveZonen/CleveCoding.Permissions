namespace CleveCoding.Kernel.Attributes;

/// <summary>
/// Markers for properties to transform (POCO) properties into data columns.
/// </summary>
[AttributeUsage(AttributeTargets.Property)]
public class DataTableObjectAttribute : Attribute;