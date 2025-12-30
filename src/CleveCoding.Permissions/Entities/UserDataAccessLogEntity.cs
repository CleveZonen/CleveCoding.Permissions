using System.ComponentModel.DataAnnotations.Schema;
using CleveCoding.Kernel.Attributes;
using CleveCoding.Kernel.Entities;

namespace CleveCoding.Permissions.Entities;

public record UserDataAccessLogEntity : Entity<long>
{
	/// <summary>
	/// Gets or sets the ID of the user whose data was accessed.
	/// </summary>
	[Encrypted, Column(TypeName = "varchar(max)")]
	public required string UserId { get; set; }

	/// <summary>
	/// Gets or sets the ID of the user who accessed the data.
	/// </summary>
	[Encrypted, Column(TypeName = "varchar(max)")]
	public required string AccessedByUserId { get; set; }

	/// <summary>
	/// Unique identifier for grouping multiple access with an single request.
	/// AccessLogs with the same AccessGroupId belong to the same request.
	/// </summary>
	public Guid AccessGroupId { get; set; }

	/// <summary>
	/// Indicates the type of action performed on the user data.
	/// </summary>
	public UserActionType Action { get; set; }

	/// <summary>
	/// Indicates the category of user data that was accessed.
	/// </summary>
	public UserDataCategory DataCategory { get; set; }

	/// <summary>
	/// DateTime when the access occurred.
	/// </summary>
	public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
