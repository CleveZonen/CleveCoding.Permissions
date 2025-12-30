using CleveCoding.Permissions;

namespace CleveCoding.PermissionsApp.Features.Mock;

/// <summary>
/// Example Request implementing IRequirePermission.
/// </summary>
public class ExportMockDetailsByIdRequest : IRequirePermission
{
	public PermissionDescription RequiredPermission => new(nameof(Mock), UserActionType.Export, "Permission to export mock details.");
}
