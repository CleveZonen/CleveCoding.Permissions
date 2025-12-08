using CleveCoding.Permissions;
using CleveCoding.Permissions.Models;

namespace CleveCoding.PermissionsApp.Features.Item;

/// <summary>
/// Example Request implementing IRequirePermission.
/// </summary>
public class GetItemsForIndexRequest : IRequirePermission
{
	public PermissionDescription RequiredPermission => new(nameof(Item), UserActionType.ViewIndex, "Access to items index.");
}
