namespace CleveCoding.Permissions.Models;

public record UserPermissionAudit
{
	/// <summary>
	/// Gets the unique identifier for the user associated with this entity.
	/// </summary>
	public string? UserId { get; init; }

	/// <summary>
	/// Gets the unique identifier of the associated role.
	/// </summary>
	public string? RoleId { get; init; }

	/// <summary>
	/// Gets the the name of the resource the permission is associated with.
	/// </summary>
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
	public DateTime CreatedAt { get; init; }

	/// <summary>
	/// Get unique identifier of the user that triggered the audit.
	/// </summary>
	public string CreatedBy { get; init; } = null!;
}
