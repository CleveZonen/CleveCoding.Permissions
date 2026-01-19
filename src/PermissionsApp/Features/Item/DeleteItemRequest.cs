using CleveCoding.Permissions;

namespace CleveCoding.PermissionsApp.Features.Item;

/// <summary>
/// Example Request implementing IRequirePermission.
/// </summary>
public class DeleteItemRequest : IRequirePermission
{
	public static PermissionDescription RequiredPermission => new()
	{
		Resource = nameof(Item),
		Action = UserActionType.Delete,
		Description = "Permission to delete items."
	};
}
