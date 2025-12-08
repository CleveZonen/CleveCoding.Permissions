namespace CleveCoding.Kernel.Extensions;

public static class DateTimeExtensions
{
	private const string DateTimeFormat = "dd-MM-yyyy HH:mm";
	private const string DateFormat = "dd-MM-yyyy";
	private const string TimeFormat = "HH:mm";

	public static string? ToLocalFormat(this DateTime? source, bool displayTime = true)
	{
		return source?.ToLocalFormat(displayTime);
	}

	public static string? ToLocalFormat(this DateTime source, bool displayTime = true)
	{
		if (source.Kind == DateTimeKind.Unspecified)
		{
			source = DateTime.SpecifyKind(source, DateTimeKind.Utc);
		}

		return source.ToLocalTime().ToString(displayTime ? DateTimeFormat : DateFormat);
	}

	public static string? ToLocalTimeFormat(this DateTime? source)
	{
		return source?.ToLocalTimeFormat();
	}

	public static string? ToLocalTimeFormat(this DateTime source)
	{
		if (source.Kind == DateTimeKind.Unspecified)
		{
			source = DateTime.SpecifyKind(source, DateTimeKind.Utc);
		}

		return source.ToLocalTime().ToString(TimeFormat);
	}
}