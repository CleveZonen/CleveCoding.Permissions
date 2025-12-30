using System.ComponentModel.DataAnnotations.Schema;
using CleveCoding.Kernel.Attributes;

namespace CleveCoding.Permissions.Entities;

/// <summary>
/// Simple audit class that audits the changes on the HasAccess-property.
/// </summary>
public record UserPermissionAuditEntity
{
	public Guid Id { get; set; } = Guid.NewGuid();

	[Encrypted, Column(TypeName = "varchar(max)")]
	public string? UserId { get; init; }

	[Encrypted, Column(TypeName = "varchar(max)")]
	public string? RoleId { get; init; }

	[Encrypted, Column(TypeName = "varchar(max)")]
	public string Resource { get; init; } = null!;

	public UserActionType Action { get; init; }

	public bool OldValue { get; init; }

	/// <summary>
	/// DateTime the change has occured.
	/// </summary>
	public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

	/// <summary>
	/// User that triggered the audit.
	/// </summary>
	public string CreatedBy { get; set; } = null!;
}
