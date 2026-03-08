using CleveCoding.Permissions.Attributes;
using CleveCoding.Permissions.Configurations;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace CleveCoding.Permissions.Web.Components;

/// <summary>
/// The PermissionRouteView secures pages 
/// decorated with the RequirePermission-Attribute.
/// </summary>
public class PermissionRouteView : RouteView
{
	[Inject]
	public IUserAccessor UserAccessor { get; set; } = null!;

	[Inject]
	public IPermissionEvaluator PermissionEvaluator { get; set; } = null!;

	[Inject]
	public NavigationManager NavigationManager { get; set; } = null!;

	[Inject]
	public PermissionConfigurations PermissionConfigurations { get; set; } = null!;

	protected override void Render(RenderTreeBuilder builder)
	{
		var pageType = RouteData.PageType;

		// detect if the current route is going to the forbidden page.
		if (!string.IsNullOrWhiteSpace(PermissionConfigurations.ErrorPageUrl) &&
			!string.IsNullOrWhiteSpace(RouteData.Template) &&
			RouteData.Template.Equals(PermissionConfigurations.ErrorPageUrl, StringComparison.CurrentCultureIgnoreCase))
		{
			base.Render(builder);
			return;
		}

		var currentUser = UserAccessor.CurrentUser!;

		// administrators have access to everything.
		if (UserAccessor.IsAdmin(currentUser))
		{
			base.Render(builder);
			return;
		}

		// get the required permissions for current page.
		var attrs = pageType.GetCustomAttributes(typeof(RequirePermissionAttribute), inherit: true)
							.Cast<RequirePermissionAttribute>()
							.ToList();

		// treat it like a normal page when no RequirePermission-attributes are found.
		if (attrs.Count == 0)
		{
			base.Render(builder);
			return;
		}

		// if any of the attr has AdminOnlyAccess, forward to error page.
		if (attrs.Any(x => x.AdminOnlyAccess))
		{
			NavigationManager.NavigateTo(PermissionConfigurations.ErrorPageUrl, true);
			return;
		}

		// access needs to be granted through the PermissionEvaluator.
		bool hasAccess = attrs.All(attr => PermissionEvaluator.HasPermission(new PermissionDescription
		{
			Action = attr.Action,
			ActionId = attr.ActionId,
			Resource = attr.Resource,
			Description = string.Empty,
		}));

		if (!hasAccess)
		{
			NavigationManager.NavigateTo(PermissionConfigurations.ErrorPageUrl, true);
			return;
		}

		base.Render(builder);
	}
}

