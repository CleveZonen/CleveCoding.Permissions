using CleveCoding.Permissions.Exceptions;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Components.Web;

namespace CleveCoding.Permissions.Web.Components;

public class PermissionErrorBoundary : ErrorBoundary
{
	protected override void BuildRenderTree(RenderTreeBuilder builder)
	{
		if (CurrentException is ForbiddenException forbidden)
		{
			builder.OpenElement(0, "div");
			builder.AddAttribute(1, "class", "alert alert-danger");

			builder.OpenElement(2, "h3");
			builder.AddContent(3, "Access Denied");
			builder.CloseElement();

			builder.OpenElement(4, "p");
			builder.AddContent(5, forbidden.Message ?? "Access denied. You do not have permission to view this content.");
			builder.CloseElement();

			builder.CloseElement();
		}
		else
		{
			base.BuildRenderTree(builder);
		}
	}

	protected override bool ShouldRender() => true;
}
