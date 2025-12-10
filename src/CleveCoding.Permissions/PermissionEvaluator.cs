using CleveCoding.Permissions.Exceptions;
using CleveCoding.Permissions.Models;

namespace CleveCoding.Permissions;

public interface IPermissionEvaluator
{
	/// <summary>
	/// Verify if the CurrentUser has permissions to access the resource.
	/// </summary>
	/// <param name="permissionDescription"></param>
	/// <exception cref="ForbiddenException">Thrown when CurrentUser is null.</exception>
	/// <returns></returns>
	bool HasPermission(PermissionDescription permissionDescription);

	/// <summary>
	/// Verify if the given user has permissions to access the resource.
	/// </summary>
	/// <param name="user"></param>
	/// <param name="permissionDescription"></param>
	/// <returns></returns>
	bool HasPermission(UserAccount user, PermissionDescription permissionDescription);
}

/// <summary>
/// </summary>
/// <param name="accessor"></param>
public class PermissionEvaluator(IUserAccessor accessor) : IPermissionEvaluator
{
	/// <inheritdoc/>
	public bool HasPermission(PermissionDescription permissionDescription)
	{
		var user = accessor.CurrentUser
			?? throw new ForbiddenException("Unauthorized user access - unkown user.")
			{
				Resource = permissionDescription.Resource,
				Action = permissionDescription.Action
			};

		// skip check if the user is admin.
		if (accessor.IsAdmin(user))
		{
			return true;
		}

		// check the permission for the user.
		return user.HasPermission(permissionDescription);
	}

	/// <inheritdoc/>
	public bool HasPermission(UserAccount user, PermissionDescription permissionDescription)
	{
		// skip check if the user is admin.
		if (accessor.IsAdmin(user))
		{
			return true;
		}

		// check the permission for the user.
		return user.HasPermission(permissionDescription);
	}
}
