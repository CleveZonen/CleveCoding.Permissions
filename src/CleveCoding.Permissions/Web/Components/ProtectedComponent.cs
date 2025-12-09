using CleveCoding.Permissions.Attributes;
using CleveCoding.Permissions.Exceptions;
using Microsoft.AspNetCore.Components;

namespace CleveCoding.Permissions.Web.Components;

/// <summary>
/// Protected component secured with permissions. 
/// Renders PermissionErrorBoundary-component if access is denied.
/// 
/// @ForbiddenException
///		Gets handled by CleveCoding.Permissions.Middleware.ForbiddenExceptionHandler
/// </summary>
public abstract class ProtectedComponent : ComponentBase
{
	[Inject] public IUserAccessor UserAccessor { get; set; } = null!;
	[Inject] public IPermissionEvaluator PermissionEvaluator { get; set; } = null!;

	protected override void OnInitialized()
	{
		var user = UserAccessor.CurrentUser
			?? throw new ForbiddenException("Unauthorized user access - unkown user.");

		// skip check if the user is admin.
		if (UserAccessor.IsAdmin(user))
		{
			return;
		}

		// get the permission attributes
		var attrs = GetType().GetCustomAttributes(typeof(RequirePermissionAttribute), inherit: true)
						.Cast<RequirePermissionAttribute>();

		// check if current user has permission to access the resource
		// if the user has permission for atleast one of them, access is granted
		if (!attrs.Any(x => PermissionEvaluator.HasPermission(new(x.Resource, x.Action))))
		{
			throw new ForbiddenException();
		}
	}
}
