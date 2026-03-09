using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace CleveCoding.Permissions.Web.Components;

/// <summary>
/// Use the ProtectedArea to secure on Component-level.
/// </summary>
public class ProtectedArea : ComponentBase
{
	[Inject]
	protected IUserAccessor UserAccessor { get; set; } = null!;

	[Inject]
	protected IPermissionEvaluator PermissionEvaluator { get; set; } = null!;

	/// <summary>
	/// The permission resource to check.
	/// </summary>
	[Parameter]
	public string Resource { get; set; } = null!;

	/// <summary>
	/// The permission action to check.
	/// </summary>
	[Parameter]
	public UserActionType Action { get; set; }

	/// <summary>
	/// The permission action id to check.
	/// </summary>
	[Parameter]
	public string? ActionId { get; set; }

	/// <summary>
	/// The permission description containing the resource, action and actionId.
	/// </summary>
	[Parameter]
	public PermissionDescription? PermissionDescription { get; set; }

	/// <summary>
	/// Render only when the user is an admin.
	/// </summary>
	[Parameter]
	public bool AdminAccessRequired { get; set; }

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

		var currentUser = UserAccessor.CurrentUser!;

		// administrators have access to everything.
		if (UserAccessor.IsAdmin(currentUser))
		{
			IsAuthorized = true;
			return;
		}

		// block further access if AdminOnlyAccess-level access required.
		if (AdminAccessRequired)
		{
			IsAuthorized = false;
			return;
		}

		IsAuthorized = PermissionEvaluator.HasPermission(PermissionDescription is not null
			? PermissionDescription
			: new PermissionDescription
			{
				Action = Action,
				ActionId = ActionId,
				Resource = Resource,
				AdminAccessRequired = AdminAccessRequired,
				Description = string.Empty,
			});
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
