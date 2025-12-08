namespace CleveCoding.Kernel.Attributes;

[AttributeUsage(AttributeTargets.Property)]
public sealed class AuditChangedAttribute(string displayName) : Attribute
{
    public string DisplayName => displayName;
}