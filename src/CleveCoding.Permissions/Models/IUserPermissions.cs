namespace CleveCoding.Permissions.Models;

public interface IUserPermissions
{
	/// <summary>
	/// Mixed user- and role permissions applicable for current user.
	/// </summary>
	IEnumerable<UserPermission>? Permissions { get; set; }

	/// <summary>
	/// Verify if the the user has permission for the given resource and action.
	/// </summary>
	/// <param name="resource"></param>
	/// <param name="action"></param>
	/// <returns></returns>
	bool HasPermission(string resource, UserActionType action);

	/// <summary>
	/// Verify if the the user has permission for the given permission description.
	/// </summary>
	/// <param name="permissionDescription"></param>
	/// <returns></returns>
	bool HasPermission(PermissionDescription permissionDescription);
}
