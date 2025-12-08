#region

using System.Text;
using Microsoft.AspNetCore.Components;

#endregion

namespace CleveCoding.Kernel.Extensions;

public static class StringExtensions
{
	/// <summary>
	///     Display the text as HTML markup.
	/// </summary>
	/// <param name="source"></param>
	/// <returns></returns>
	public static MarkupString AsMarkup(this string? source)
	{
		return (MarkupString)(source ?? "");
	}

	/// <summary>
	///     Only returns numbers, a-z upper- and lowercase, dash '-' and underscore '_'
	/// </summary>
	/// <param name="source"></param>
	/// <returns></returns>
	public static string SanitizeAlphaNum(this string source)
	{
		return new([.. source.Where(x => x is >= '0' and <= '9'
			or >= 'A' and <= 'Z'
			or >= 'a' and <= 'z'
			or '-'
			or '_'
		)]);
	}

	/// <summary>
	///     Truncate a string until the maxLength and replace it with the suffix.
	/// </summary>
	/// <param name="source"></param>
	/// <param name="maxLength"></param>
	/// <param name="suffix"></param>
	/// <returns></returns>
	public static string? Truncate(this string? source, int maxLength, string? suffix = "..")
	{
		return string.IsNullOrWhiteSpace(source)
			? source
			: source.Length > maxLength
				? $"{source[..maxLength]}{suffix}"
				: source;
	}

	#region String Template Formatting

	/// <summary>
	///     Parse the source template. The target model will be scanned based on the tags
	///     and the source will be formatted appropriately.
	/// </summary>
	/// <typeparam name="T">Class to scan for formatting.</typeparam>
	/// <param name="source">Template with tags.</param>
	/// <param name="target">The Model to scan.</param>
	/// <param name="startTagPattern"></param>
	/// <param name="endTagPattern"></param>
	/// <returns>New string with the model parsed into the template.</returns>
	public static string ParseTemplate<T>(this string source, T target, string startTagPattern = "[", string endTagPattern = "]")
		where T : class
	{
		var foundTags = source.ExtractTags(startTagPattern, endTagPattern).ToArray();
		var tagValues = GetValuesFromObject(foundTags, target);

		foundTags = [.. foundTags.Where(tagValues.ContainsKey)];
		var formatTemplate = source.ConvertTagsForFormatting(foundTags, startTagPattern, endTagPattern);
		var args = foundTags.Select(x => tagValues[x]);

		return string.Format(formatTemplate.ToString(), [.. args]);
	}

	/// <summary>
	///     Convert the source string with tags to be usable in string.Format().
	/// </summary>
	/// <param name="source"></param>
	/// <param name="foundTags"></param>
	/// <param name="startTagPattern"></param>
	/// <param name="endTagPattern"></param>
	/// <returns>StringBuilder that holds a string suitable for string.Format()</returns>
	private static StringBuilder ConvertTagsForFormatting(this string source, IEnumerable<string> foundTags, string startTagPattern, string endTagPattern)
	{
		var index = 0;
		var formattedSource = new StringBuilder(source);
		foreach (var tag in foundTags)
		{
			string? formatting = null;
			if (tag.Contains(':'))
			{
				formatting = tag.Split(':')[1];
			}

			formattedSource.Replace($"{startTagPattern}{tag}{endTagPattern}",
				formatting is null ? $"{{{index}}}" : $"{{{index}:{formatting}}}");

			index++;
		}

		return formattedSource;
	}

	/// <summary>
	///     Find and extract the tags found in the source string.
	/// </summary>
	/// <param name="source"></param>
	/// <param name="startTagPattern"></param>
	/// <param name="endTagPattern"></param>
	/// <returns>Unique list of tags.</returns>
	private static List<string> ExtractTags(this string source, string startTagPattern, string endTagPattern)
	{
		var foundTags = new List<string>();
		var startIndex = 0;
		while (startIndex < source.Length)
		{
			var openBracketIndex = source.IndexOf(startTagPattern, startIndex, StringComparison.Ordinal);
			if (openBracketIndex < 0) break;

			var closeBracketIndex = source.IndexOf(endTagPattern, openBracketIndex + 1, StringComparison.Ordinal);
			if (closeBracketIndex < 0) break;

			var tag = source.Substring(openBracketIndex + 1, closeBracketIndex - openBracketIndex - 1);
			if (!foundTags.Contains(tag))
			{
				foundTags.Add(tag);
			}

			startIndex = closeBracketIndex + 1;
		}

		return foundTags;
	}

	/// <summary>
	///     Get the values in the target model to replace the tags with.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <param name="paths"></param>
	/// <param name="target"></param>
	/// <returns>Dictionary where Key => Tag and Value => Content</returns>
	private static Dictionary<string, object> GetValuesFromObject<T>(IEnumerable<string> paths, T target)
	{
		var foundValues = new Dictionary<string, object>();
		if (target is null)
		{
			return foundValues;
		}

		var targetType = typeof(T);
		foreach (var path in paths)
		{
			var currentPropertyName = path;

			// strip formatting
			if (currentPropertyName.Contains(':'))
			{
				currentPropertyName = currentPropertyName.Split(':')[0];
			}

			if (!currentPropertyName.Contains('.'))
			{
				var property = targetType.GetProperty(currentPropertyName);
				if (property == null)
				{
					continue;
				}

				foundValues[path] = property.GetValue(target) ?? "";
			}
			else
			{
				foundValues[path] = GetNestedPropertyValue(target, currentPropertyName) ?? "";
			}
		}

		return foundValues;
	}

	/// <summary>
	///     Get the value of the property path in the source.
	/// </summary>
	/// <param name="source"></param>
	/// <param name="propertyPath"></param>
	/// <returns></returns>
	private static object? GetNestedPropertyValue(object source, string propertyPath)
	{
		var properties = propertyPath.Split('.');
		var value = source;

		foreach (var property in properties)
		{
			var propertyInfo = value.GetType().GetProperty(property);
			if (propertyInfo == null)
			{
				return null;
			}

			value = propertyInfo.GetValue(value);
			if (value == null)
			{
				return null;
			}
		}

		return value;
	}

	#endregion String Template Formatting
}