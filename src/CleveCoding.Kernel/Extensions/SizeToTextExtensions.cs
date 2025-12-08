namespace CleveCoding.Kernel.Extensions;

public static class SizeToTextExtensions
{
	public enum SizeUnit { Byte, KB, MB, GB, TB, PB, EB, ZB, YB }

	public static string GetSizeText(this long source, SizeUnit unit)
	{
		return $"{source / Math.Pow(1024, (long)unit):0.##} {unit}";
	}

	public static string GetSizeText(this ulong source, SizeUnit unit)
	{
		return $"{source / Math.Pow(1024, (long)unit):0.##} {unit}";
	}

	public static string GetSizeText(this int source, SizeUnit unit)
	{
		return $"{source / Math.Pow(1024, (long)unit):0.##} {unit}";
	}
}