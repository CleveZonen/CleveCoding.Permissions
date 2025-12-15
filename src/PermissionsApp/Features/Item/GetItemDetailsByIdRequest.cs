using CleveCoding.Permissions;
using CleveCoding.Permissions.Models;

namespace CleveCoding.PermissionsApp.Features.Item;

/// <summary>
/// Example Request implementing IRequirePermission.
/// </summary>
public class GetItemDetailsByIdRequest : IRequirePermission
{
	public PermissionDescription RequiredPermission => new(nameof(Item), UserActionType.ViewDetails, "Access to item details.");
}
