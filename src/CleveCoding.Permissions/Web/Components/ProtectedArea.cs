using CleveCoding.Permissions.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace CleveCoding.Permissions.Web.Components;

public class ProtectedArea : ComponentBase
{
	[Inject]
	protected IUserAccessor UserAccessor { get; set; } = null!;

	[Inject]
	protected IPermissionEvaluator PermissionEvaluator { get; set; } = null!;

	/// <summary>
	/// The permission resource to check.
	/// </summary>
	[Parameter, EditorRequired]
	public string Resource { get; set; } = null!;

	/// <summary>
	/// The permission action to check.
	/// </summary>
	[Parameter, EditorRequired]
	public UserActionType Action { get; set; }

	/// <summary>
	/// Content to render when authorized.
	/// </summary>
	[Parameter]
	public RenderFragment? ChildContent { get; set; }

	/// <summary>
	/// Content to render when not authorized.
	/// </summary>
	[Parameter]
	public RenderFragment? NotAuthorized { get; set; }

	protected bool IsAuthorized { get; set; }

	protected override void OnParametersSet()
	{
		// Do NOT block rendering during prerender if user not loaded yet.
		// Instead, treat as unauthorized until UserContextInitializer finalizes.
		if (UserAccessor.CurrentUser == null)
		{
			IsAuthorized = false;
			return;
		}

		IsAuthorized = PermissionEvaluator.HasPermission(new(Resource, Action));
	}

	protected override void BuildRenderTree(RenderTreeBuilder builder)
	{
		if (IsAuthorized)
		{
			builder.AddContent(0, ChildContent);
			return;
		}

		if (NotAuthorized != null)
		{
			builder.AddContent(0, NotAuthorized);
		}
	}
}
