using CleveCoding.Permissions;

namespace CleveCoding.PermissionsApp.Features.Mock;

/// <summary>
/// Example Request implementing IRequirePermission.
/// </summary>
public class GetMockDetailsByIdRequest : IRequirePermission
{
	public PermissionDescription RequiredPermission => new()
	{
		Resource = nameof(Mock),
		Action = UserActionType.ViewDetails,
		Description = "Access to mock details."
	};
}
