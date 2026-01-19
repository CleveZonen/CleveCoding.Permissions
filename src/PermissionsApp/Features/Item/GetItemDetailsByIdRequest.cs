using CleveCoding.Permissions;

namespace CleveCoding.PermissionsApp.Features.Item;

/// <summary>
/// Example Request implementing IRequirePermission.
/// </summary>
public class GetItemDetailsByIdRequest(int Id) : IRequirePermission
{
	public static PermissionDescription RequiredPermission => new()
	{
		Resource = nameof(Item),
		Action = UserActionType.ViewDetails,
		Description = "Access to items details.",
		DataCategories = [
			UserDataCategory.PersonalIdentity,
			UserDataCategory.ContactInformation
		]
	};
}
