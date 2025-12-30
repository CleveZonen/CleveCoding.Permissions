namespace CleveCoding.Permissions.Attributes;

/// <summary>
/// Use the RequirePermission Attribute to protect on Component-level
/// and inheriting from CleveCoding.Permissions.Web.Components.ProtectedComponent.
/// </summary>
/// <param name="resource"></param>
/// <param name="action"></param>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public class RequirePermissionAttribute(string resource, UserActionType action) : Attribute
{
	public string Resource { get; } = resource;
	public UserActionType Action { get; } = action;
}
