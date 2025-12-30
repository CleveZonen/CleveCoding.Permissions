namespace CleveCoding.Permissions.Models;

public record UserPermission
{
	/// <summary>
	/// Gets the unique identifier for the user associated with this entity.
	/// </summary>
	public string? UserId { get; set; }

	/// <summary>
	/// Gets the unique identifier of the associated role.
	/// </summary>
	public string? RoleId { get; set; }

	/// <summary>
	/// Should be exact match with IRequirePermission.PermissionDescription.Resource
	/// </summary>
	public string Resource { get; set; } = null!;

	/// <summary>
	/// Gets the action that gets performed.
	/// </summary>
	public UserActionType Action { get; set; }

	/// <summary>
	/// Indicates whether the user or role has access to the specified resource and action.
	/// </summary>
	public bool HasAccess { get; set; }

	public DateTime CreatedAt { get; set; }
	public string CreatedBy { get; set; } = null!;
}
