using CleveCoding.Permissions.Attributes;
using CleveCoding.Permissions.Exceptions;
using Microsoft.AspNetCore.Components;

namespace CleveCoding.Permissions.Web.Components;

/// <summary>
/// Protected component secured with permissions. 
/// Throws ForbiddenException if access is denied.
/// </summary>
public abstract class ProtectedComponent : ComponentBase
{
	[Inject]
	public IUserAccessor UserAccessor { get; set; } = default!;

	protected override void OnInitialized()
	{
		if (UserAccessor.CurrentUser is null)
		{
			throw new ForbiddenException();
		}

		// skip check if the user is admin.
		if (UserAccessor.IsAdmin(UserAccessor.CurrentUser))
		{
			base.OnInitialized();
			return;
		}

		// get the permission attributes
		var attrs = GetType().GetCustomAttributes(typeof(RequirePermissionAttribute), inherit: true)
						.Cast<RequirePermissionAttribute>();

		// check if current user has permission to access the resource
		// if the user has permission for atleast one of them, access is granted
		var hasAccess = false;
		foreach (var attr in attrs)
		{
			if (UserAccessor.CurrentUser.HasPermission(attr.Resource, attr.Action))
			{
				hasAccess = true;
			}
		}

		if (!hasAccess)
		{
			throw new ForbiddenException();
		}

		base.OnInitialized();
	}
}
