using CleveCoding.Permissions.Attributes;
using CleveCoding.Permissions.Exceptions;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace CleveCoding.Permissions.Web.Components;

public class PermissionRouteView : RouteView
{
	[Inject] public IUserAccessor UserAccessor { get; set; } = default!;
	[Inject] public IPermissionEvaluator PermissionEvaluator { get; set; } = default!;

	protected override void Render(RenderTreeBuilder builder)
	{
		var pageType = RouteData.PageType;
		var attrs = pageType.GetCustomAttributes(typeof(RequirePermissionAttribute), inherit: true)
							.Cast<RequirePermissionAttribute>()
							.ToList();

		// default access to true,
		// only when RequirePermission is used,
		// access needs to be restricted.
		bool hasAccess = true;

		// if RequirePermission-attributes are found,
		// evaluate the user permissions for access.
		if (attrs.Count != 0)
		{
			hasAccess = attrs.Any(attr => PermissionEvaluator.HasPermission(new(attr.Resource, attr.Action)));
		}

		if (!hasAccess)
		{
			throw new ForbiddenException();
		}
		else
		{
			base.Render(builder);
		}
	}
}

