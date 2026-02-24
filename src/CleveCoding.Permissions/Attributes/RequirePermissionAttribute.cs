namespace CleveCoding.Permissions.Attributes;

/// <summary>
/// Use the RequirePermission Attribute to protect on Component-level
/// and inheriting from CleveCoding.Permissions.Web.Components.ProtectedComponent.
/// </summary>
/// <param name="resource"></param>
/// <param name="action"></param>
/// <param name="actionId"></param>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public class RequirePermissionAttribute(string resource, UserActionType action, string? actionId) : Attribute
{
	public string Resource { get; } = resource;
	public string? ActionId { get; } = actionId;
	public UserActionType Action { get; } = action;
}
