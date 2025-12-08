namespace CleveCoding.Kernel.Attributes;

[AttributeUsage(AttributeTargets.Property)]
public class IsUtcAttribute(bool isUtc = true) : Attribute
{
	public bool IsUtc { get; } = isUtc;
}
