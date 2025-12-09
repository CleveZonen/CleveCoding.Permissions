using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

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
	[Inject]
	public IUserAccessor UserAccessor { get; set; } = default!;

	[Parameter] public RenderFragment? ChildContent { get; set; }

	protected override void BuildRenderTree(RenderTreeBuilder builder)
	{
		builder.OpenComponent<PermissionErrorBoundary>(0);
		builder.AddAttribute(1, nameof(PermissionErrorBoundary.ChildContent), ChildContent);
		builder.CloseComponent();
	}
}
