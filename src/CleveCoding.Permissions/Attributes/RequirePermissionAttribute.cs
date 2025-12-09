using CleveCoding.Permissions.Models;

namespace CleveCoding.Permissions.Attributes;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public class RequirePermissionAttribute(string resource, UserActionType action) : Attribute
{
	public string Resource { get; } = resource;
	public UserActionType Action { get; } = action;
}
