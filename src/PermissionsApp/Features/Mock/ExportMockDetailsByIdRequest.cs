using CleveCoding.Permissions;

namespace CleveCoding.PermissionsApp.Features.Mock;

/// <summary>
/// Example Request implementing IRequirePermission.
/// </summary>
public class ExportMockDetailsByIdRequest : IRequirePermission
{
	public PermissionDescription RequiredPermission => new()
	{
		Resource = nameof(Mock),
		Action = UserActionType.Export,
		Description = "Permission to export mock details."
	};
}
