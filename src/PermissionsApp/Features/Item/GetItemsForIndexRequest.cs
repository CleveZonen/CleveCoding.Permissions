using CleveCoding.Permissions;

namespace CleveCoding.PermissionsApp.Features.Item;

/// <summary>
/// Example Request implementing IRequirePermission.
/// </summary>
public class GetItemsForIndexRequest : IRequirePermission
{
	public static PermissionDescription RequiredPermission => new()
	{
		Resource = nameof(Item),
		Action = UserActionType.ViewIndex,
		Description = "Access to items index."
	};
}

/// <summary>
/// Example Request implementing IRequirePermission.
/// </summary>
public class GetItemsForSelectorRequest : IRequirePermission
{
	public static PermissionDescription RequiredPermission => new()
	{
		Resource = nameof(Item),
		Action = UserActionType.ViewIndex,
		ActionId = "Selector",
		Description = "Access to items list."
	};
}
