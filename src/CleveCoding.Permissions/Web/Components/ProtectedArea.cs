using CleveCoding.Permissions.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace CleveCoding.Permissions.Web.Components;

public class ProtectedArea : ComponentBase
{
	[Inject]
	protected IUserAccessor UserAccessor { get; set; } = default!;

	/// <summary>
	/// The permission resource to check.
	/// </summary>
	[Parameter, EditorRequired]
	public string Resource { get; set; } = default!;

	/// <summary>
	/// The permission action to check.
	/// </summary>
	[Parameter, EditorRequired]
	public UserActionType Action { get; set; }

	/// <summary>
	/// Child content to render when authorized.
	/// </summary>
	[Parameter]
	public RenderFragment? ChildContent { get; set; }

	protected bool IsAuthorized { get; set; }

	protected override void OnParametersSet()
	{
		var user = UserAccessor.CurrentUser;

		// Do NOT block rendering during prerender if user not loaded yet.
		// Instead, treat as unauthorized until UserContextInitializer finalizes.
		if (user == null)
		{
			IsAuthorized = false;
			return;
		}

		// Administrators bypass permissions
		if (UserAccessor.IsAdmin(user))
		{
			IsAuthorized = true;
			return;
		}

		IsAuthorized = user.HasPermission(Resource, Action);
	}

	protected override void BuildRenderTree(RenderTreeBuilder builder)
	{
		if (IsAuthorized)
		{
			builder.AddContent(0, ChildContent);
		}
	}
}
