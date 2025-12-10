using CleveCoding.Permissions.Attributes;
using CleveCoding.Permissions.Configurations;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace CleveCoding.Permissions.Web.Components;

public class PermissionRouteView : RouteView
{
	[Inject] public IUserAccessor UserAccessor { get; set; } = null!;
	[Inject] public IPermissionEvaluator PermissionEvaluator { get; set; } = null!;
	[Inject] public NavigationManager NavigationManager { get; set; } = null!;
	[Inject] public PermissionConfigurations PermissionConfigurations { get; set; } = null!;

	protected override void Render(RenderTreeBuilder builder)
	{
		var pageType = RouteData.PageType;

		// detect if the current route is going to the forbidden page
		if (!string.IsNullOrWhiteSpace(PermissionConfigurations.ErrorPageUrl) && !string.IsNullOrWhiteSpace(RouteData.Template))
		{
			if (RouteData.Template.Equals(PermissionConfigurations.ErrorPageUrl, StringComparison.CurrentCultureIgnoreCase))
			{
				base.Render(builder);
				return;
			}
		}

		// get the required permissions for current page.
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
			NavigationManager.NavigateTo(PermissionConfigurations.ErrorPageUrl ?? "/errors/forbidden", true);
			return;
		}

		base.Render(builder);
	}
}

