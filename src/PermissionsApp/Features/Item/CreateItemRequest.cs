using CleveCoding.Permissions;

namespace CleveCoding.PermissionsApp.Features.Item;

/// <summary>
/// Example Request implementing IRequirePermission.
/// </summary>
public class CreateItemRequest : IRequirePermission
{
	public static PermissionDescription RequiredPermission => new()
	{
		Resource = nameof(Item),
		Action = UserActionType.Create,
		Description = "Permission to create items."
	};
}
