using CleveCoding.Permissions;
using CleveCoding.Permissions.Models;

namespace CleveCoding.PermissionsApp.Features.Item;

/// <summary>
/// Example Request implementing IRequirePermission.
/// </summary>
public class DeleteItemRequest : IRequirePermission
{
	public PermissionDescription RequiredPermission => new(nameof(Item), UserActionType.Delete, "Permission to delete items.");
}
