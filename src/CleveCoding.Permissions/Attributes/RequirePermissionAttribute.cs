namespace CleveCoding.Permissions.Attributes;

/// <summary>
/// Use the RequirePermission Attribute to secure on Page-level.
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public class RequirePermissionAttribute : Attribute
{
	public string Resource { get; }
	public string? ActionId { get; }
	public UserActionType Action { get; }

	public bool AdminAccessRequired { get; set; }

	public PermissionDescription? PermissionDescription { get; set; }

	public RequirePermissionAttribute(string resource, UserActionType action, string? actionId)
	{
		Resource = resource;
		ActionId = actionId;
		Action = action;
	}

	public RequirePermissionAttribute(bool adminAccessRequired)
	{
		Resource = "";
		Action = UserActionType.None;
		AdminAccessRequired = adminAccessRequired;
	}

	public RequirePermissionAttribute(Type requestType)
	{
		if (!typeof(IRequirePermission).IsAssignableFrom(requestType))
		{
			throw new ArgumentException(
				$"{requestType.Name} must implement IRequirePermission");
		}

		PermissionDescription = (PermissionDescription)requestType
				.GetProperty(nameof(IRequirePermission.RequiredPermission))!
				.GetValue(null)!;

		Resource = PermissionDescription.Resource;
		Action = PermissionDescription.Action;
		ActionId = PermissionDescription.ActionId;
		AdminAccessRequired = PermissionDescription.AdminAccessRequired;
	}
}
