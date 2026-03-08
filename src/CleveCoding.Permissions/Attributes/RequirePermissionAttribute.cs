namespace CleveCoding.Permissions.Attributes;

/// <summary>
/// Use the RequirePermission Attribute to protect on Component-level
/// and inheriting from CleveCoding.Permissions.Web.Components.ProtectedComponent.
/// </summary>
/// <param name="resource"></param>
/// <param name="action"></param>
/// <param name="actionId"></param>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public class RequirePermissionAttribute : Attribute
{
	public string Resource { get; }
	public string? ActionId { get; }
	public UserActionType Action { get; }
	public bool AdminOnlyAccess { get; set; }

	public RequirePermissionAttribute(string resource, UserActionType action, string? actionId)
	{
		Resource = resource;
		ActionId = actionId;
		Action = action;
	}

	public RequirePermissionAttribute(bool adminOnlyAccess)
	{
		Resource = "";
		Action = UserActionType.None;
		AdminOnlyAccess = adminOnlyAccess;
	}
}
