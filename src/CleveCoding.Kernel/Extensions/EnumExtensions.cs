#region

using System.ComponentModel.DataAnnotations;

#endregion

namespace CleveCoding.Kernel.Extensions;

public static class EnumExtensions
{
	/// <summary>
	/// Get the name-value from the Display-attribute.
	/// If no attribute is found, the value-string is returned.
	/// </summary>
	/// <param name="source"></param>
	/// <returns></returns>
	public static string GetName(this Enum source)
	{
		var field = source.GetType().GetField(source.ToString());
		if (field == null)
		{
			return source.ToString();
		}

		return Attribute.GetCustomAttribute(field, typeof(DisplayAttribute)) is DisplayAttribute attribute
			? attribute.Name ?? source.ToString()
			: source.ToString();
	}
	/// <summary>
	/// Get the description-value from the Display-attribute.
	/// </summary>
	/// <param name="source"></param>
	/// <returns></returns>
	public static string? GetDescription(this Enum source)
	{
		var field = source.GetType().GetField(source.ToString());
		if (field == null)
		{
			return null;
		}

		return Attribute.GetCustomAttribute(field, typeof(DisplayAttribute)) is DisplayAttribute attribute
			? attribute.Description ?? source.ToString()
			: source.ToString();
	}

	/// <summary>
	/// Get the group-value from the Display-attribute.
	/// </summary>
	/// <param name="source"></param>
	/// <returns></returns>
	public static string? GetGroup(this Enum source)
	{
		var field = source.GetType().GetField(source.ToString());
		if (field == null)
		{
			return null;
		}

		return Attribute.GetCustomAttribute(field, typeof(DisplayAttribute)) is DisplayAttribute attribute
			? attribute.GroupName
			: null;
	}

	/// <summary>
	/// Compare if two fields from the same enum have the same exact groupname.
	/// </summary>
	/// <param name="source"></param>
	/// <param name="compareTo">Must be from the source Enum as well</param>
	/// <returns></returns>
	public static bool EqualsGroup(this Enum source, Enum compareTo)
	{
		var sourceField = source.GetType().GetField(source.ToString());
		if (sourceField == null)
		{
			return false;
		}

		var compareField = source.GetType().GetField(compareTo.ToString());
		if (compareField == null)
		{
			return false;
		}

		if (Attribute.GetCustomAttribute(sourceField, typeof(DisplayAttribute)) is not DisplayAttribute sourceDisplayAttribute)
		{
			return false;
		}

		if (Attribute.GetCustomAttribute(compareField, typeof(DisplayAttribute)) is not DisplayAttribute compareToDisplayAttribute)
		{
			return false;
		}

		return sourceDisplayAttribute.GroupName is not null && sourceDisplayAttribute.GroupName.Equals(compareToDisplayAttribute.GroupName);
	}

	/// <summary>
	/// Get all the Enum values from an group.
	/// </summary>
	/// <param name="source"></param>
	/// <param name="groupName"></param>
	/// <returns></returns>
	public static IEnumerable<T> GetFromGroup<T>(this T source, string groupName)
		where T : Enum
	{
		return [.. Enum.GetValues(source.GetType()).Cast<T>().Where(x => x.GetGroup() is not null && x.GetGroup()!.Equals(groupName, StringComparison.InvariantCultureIgnoreCase))];
	}
}