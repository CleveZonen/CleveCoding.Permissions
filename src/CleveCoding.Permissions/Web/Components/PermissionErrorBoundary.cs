using CleveCoding.Permissions.Attributes;
using CleveCoding.Permissions.Exceptions;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Components.Web;

namespace CleveCoding.Permissions.Web.Components;

/// <summary>
/// Error boundary that handles ForbiddenException for protected components.
/// </summary>
public class PermissionErrorBoundary : ErrorBoundary
{
	[Inject]
	public IUserAccessor UserAccessor { get; set; } = default!;

	/// <summary>
	/// Wrap child content as well, so trigger an state change.
	/// </summary>
	/// <param name="exception"></param>
	/// <returns></returns>
	protected override Task OnErrorAsync(Exception exception)
	{
		if (exception is ForbiddenException)
		{
			StateHasChanged();
		}
		return base.OnErrorAsync(exception);
	}

	protected override void BuildRenderTree(RenderTreeBuilder builder)
	{
		try
		{
			// Check permissions before rendering
			var attrs = ChildContent?.GetType().GetCustomAttributes(typeof(RequirePermissionAttribute), true)
				.Cast<RequirePermissionAttribute>() ?? [];

			var currentUser = UserAccessor.CurrentUser;
			if (currentUser == null || !attrs.Any(attr => currentUser.HasPermission(attr.Resource, attr.Action)))
			{
				throw new ForbiddenException();
			}

			// If permission passed, render the child content
			builder.AddContent(0, ChildContent);
		}
		catch (ForbiddenException forbidden)
		{
			// Render fallback UI
			builder.OpenElement(1, "div");
			builder.AddAttribute(2, "class", "alert alert-danger");

			builder.OpenElement(3, "h3");
			builder.AddContent(4, "Access Denied");
			builder.CloseElement();

			builder.OpenElement(5, "p");
			builder.AddContent(6, forbidden.Message ?? "Access denied. You do not have permission to view this content.");
			builder.CloseElement();

			builder.CloseElement();
		}
		catch (Exception)
		{
			// Fallback to normal ErrorBoundary behavior for other exceptions
			base.BuildRenderTree(builder);
		}

		base.BuildRenderTree(builder);
	}

	protected override bool ShouldRender() => true;
}
