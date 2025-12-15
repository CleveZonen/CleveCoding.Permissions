namespace CleveCoding.Permissions.Models;

public record UserPermission
{
	public string? UserId { get; set; }
	public string? RoleId { get; set; }
	public DateTime CreatedAt { get; set; }
	public string CreatedBy { get; set; } = null!;

	/// <summary>
	/// Should be exact match with IRequirePermission.PermissionDescription.Resource
	/// </summary>
	public string Resource { get; set; } = null!;
	public UserActionType Action { get; set; }
	public bool HasAccess { get; set; }
}
