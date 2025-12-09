using CleveCoding.Permissions;
using CleveCoding.Permissions.Models;

namespace CleveCoding.PermissionsApp.Items;

/// <summary>
/// Example Request implementing IRequirePermission.
/// </summary>
public class GetItemsForIndexRequest : IRequirePermission
{
	public PermissionDescription RequiredPermission => new(nameof(Items), UserActionType.ViewIndex, "Access to items index.");
}
