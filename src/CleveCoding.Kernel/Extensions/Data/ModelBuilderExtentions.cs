using System.Reflection;
using CleveCoding.Kernel.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace CleveCoding.Kernel.Extensions.Data;

/// <summary>
/// Extensions to handle UTC DateTime properties.
/// </summary>
public static class ModelBuilderExtentions
{
	#region [ Encryption Attribute Handling ]

	/// <summary>
	/// Make sure every property marked with the attribute is encrypted and decrypted before database actions.
	/// </summary>
	/// <typeparam name="TField"></typeparam>
	/// <param name="modelBuilder"></param>
	/// <param name="converter"></param>
	public static void UseEncryption<TField>(this ModelBuilder modelBuilder, ValueConverter converter)
	{
		var properties = modelBuilder.Model.GetEntityTypes().SelectMany(x => x.GetProperties());
		foreach (var property in properties)
		{
			if (IsDiscriminator(property)) continue;
			if (property.ClrType != typeof(TField)) continue;

			var attributes = property.PropertyInfo?.GetCustomAttributes(typeof(EncryptedAttribute), false);
			if (attributes is null || attributes.Length == 0) continue;

			property.SetValueConverter(converter);
		}
	}

	/// <summary>
	/// A helper function to ignore EF Core Discriminator.
	/// </summary>
	/// <param name="property"></param>
	/// <returns></returns>
	private static bool IsDiscriminator(IMutableProperty property)
	{
		return property.Name == "Discriminator" || property.PropertyInfo == null;
	}

	#endregion [ Encryption Attribute Handling ]

	#region [ IsUtc Attribute Handling ]

	private const string IsUtcAnnotation = "IsUtc";
	private static readonly ValueConverter<DateTime, DateTime> UtcConverter =
		new(x => x, x => DateTime.SpecifyKind(x, DateTimeKind.Utc));

	/// <summary>
	/// All the DateTime properties are assumed to be UTC-enabled.
	/// To exclude a property as DateTimeKind.UTC, use the IsUtc-attribute.
	/// </summary>
	/// <param name="builder"></param>
	public static void ApplyUtcDateTimeConverter(this ModelBuilder builder)
	{
		foreach (var entityType in builder.Model.GetEntityTypes())
		{
			foreach (var property in entityType.GetProperties())
			{
				if (property.ClrType != typeof(DateTime) && property.ClrType != typeof(DateTime?))
				{
					continue;
				}

				if (!property.IsUtc())
				{
					continue;
				}

				property.SetValueConverter(UtcConverter);
			}
		}
	}

	/// <summary>
	/// Get the value of the IsUtc-attribute.
	/// </summary>
	/// <param name="property"></param>
	/// <returns>True as default and if no attribute has been found.</returns>
	private static bool IsUtc(this IMutableProperty property)
	{
		var attribute = property.PropertyInfo?.GetCustomAttribute<IsUtcAttribute>();
		if (attribute is not null && attribute.IsUtc)
		{
			return true;
		}

		return ((bool?)property.FindAnnotation(IsUtcAnnotation)?.Value) ?? true;
	}

	#endregion [ IsUtc Attribute Handling ]
}
