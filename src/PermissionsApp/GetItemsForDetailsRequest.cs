using CleveCoding.Permissions;
using CleveCoding.Permissions.Models;

namespace CleveCoding.PermissionsApp.Features.Item;

/// <summary>
/// Example 2 Request implementing IRequirePermission.
/// </summary>
public class GetItemsForDetailsRequest : IRequirePermission
{
	public PermissionDescription RequiredPermission => new(nameof(Item), UserActionType.ViewDetails, "Access to items details.");
}
