using CleveCoding.Permissions;

namespace CleveCoding.PermissionsApp.Features.Item;

/// <summary>
/// Example Request implementing IRequirePermission.
/// </summary>
public class UpdateItemRequest : IRequirePermission
{
	public static PermissionDescription RequiredPermission => new()
	{
		Resource = nameof(Item),
		Action = UserActionType.Update,
		Description = "Permission to update items."
	};
}
