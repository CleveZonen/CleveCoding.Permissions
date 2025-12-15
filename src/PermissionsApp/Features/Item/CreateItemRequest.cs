using CleveCoding.Permissions;
using CleveCoding.Permissions.Models;

namespace CleveCoding.PermissionsApp.Features.Item;

/// <summary>
/// Example Request implementing IRequirePermission.
/// </summary>
public class CreateItemRequest : IRequirePermission
{
	public PermissionDescription RequiredPermission => new(nameof(Item), UserActionType.Create, "Permission to create items.");
}
