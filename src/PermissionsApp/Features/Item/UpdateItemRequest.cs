using CleveCoding.Permissions;
using CleveCoding.Permissions.Models;

namespace CleveCoding.PermissionsApp.Features.Item;

/// <summary>
/// Example Request implementing IRequirePermission.
/// </summary>
public class UpdateItemRequest : IRequirePermission
{
	public PermissionDescription RequiredPermission => new(nameof(Item), UserActionType.Update, "Permission to update items.");
}
