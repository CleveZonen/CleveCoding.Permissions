using CleveCoding.Permissions;
using CleveCoding.Permissions.Models;

namespace CleveCoding.PermissionsApp.Features.Mock;

/// <summary>
/// Example Request implementing IRequirePermission.
/// </summary>
public class GetMockDetailsByIdRequest : IRequirePermission
{
	public PermissionDescription RequiredPermission => new(nameof(Mock), UserActionType.ViewDetails, "Access to mock details.");
}
