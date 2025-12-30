using System.ComponentModel.DataAnnotations.Schema;
using CleveCoding.Kernel.Attributes;

namespace CleveCoding.Permissions.Entities;

/// <summary>
/// Simple audit class that audits the changes on the HasAccess-property.
/// </summary>
public record UserPermissionAuditEntity
{
	/// <summary>
	/// Identifier of the audit entry. 
	/// </summary>
	public Guid Id { get; set; } = Guid.NewGuid();

	/// <summary>
	/// Gets the unique identifier for the user associated with this entity.
	/// </summary>
	[Encrypted, Column(TypeName = "varchar(max)")]
	public string? UserId { get; init; }

	/// <summary>
	/// Gets the unique identifier of the associated role.
	/// </summary>
	[Encrypted, Column(TypeName = "varchar(max)")]
	public string? RoleId { get; init; }

	/// <summary>
	/// Gets the the name of the resource the permission is associated with.
	/// </summary>
	[Encrypted, Column(TypeName = "varchar(max)")]
	public string Resource { get; init; } = null!;

	/// <summary>
	/// Gets the action that has been performed.
	/// </summary>
	public UserActionType Action { get; init; }

	/// <summary>
	/// Old value of the HasAccess-property.
	/// </summary>
	public bool OldValue { get; init; }

	/// <summary>
	/// DateTime the change has occured.
	/// </summary>
	public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

	/// <summary>
	/// Get unique identifier of the user that triggered the audit.
	/// </summary>
	public string CreatedBy { get; set; } = null!;
}
