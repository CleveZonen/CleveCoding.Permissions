using CleveCoding.Permissions;
using CleveCoding.Permissions.Models;

namespace CleveCoding.PermissionsApp.Items;

/// <summary>
/// Example 2 Request implementing IRequirePermission.
/// </summary>
public class GetItemsForDetailsRequest : IRequirePermission
{
	public PermissionDescription RequiredPermission => new(nameof(Items), UserActionType.ViewDetails, "Access to items details.");
}
