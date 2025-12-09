using CleveCoding.Permissions.Exceptions;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Components.Web;

namespace CleveCoding.Permissions.Web.Components;

/// <summary>
/// Error boundary that handles ForbiddenException for protected components.
/// </summary>
public class PermissionErrorBoundary : ErrorBoundary
{
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
		if (CurrentException is ForbiddenException forbidden)
		{
			// Render fallback UI
			builder.OpenElement(1, "div");
			builder.AddAttribute(2, "class", "alert alert-danger");

			builder.OpenElement(3, "h3");
			builder.AddContent(4, "Access Denied");
			builder.CloseElement();

			builder.OpenElement(5, "p");
			builder.AddContent(6, forbidden.Message);
			builder.CloseElement();

			builder.CloseElement();
			return;
		}
		else
		{
			base.BuildRenderTree(builder);
		}
	}

	protected override bool ShouldRender() => true;
}
