using CleveCoding.Permissions;

namespace CleveCoding.PermissionsApp.Features.Item;

/// <summary>
/// Example Request implementing IRequirePermission.
/// </summary>
public class GetItemDetailsByIdRequest : IRequirePermission
{
	public PermissionDescription RequiredPermission => new(nameof(Item), UserActionType.ViewDetails, "Access to item details.")
	{
		DataCategories = [
			UserDataCategory.PersonalIdentity,
			UserDataCategory.ContactInformation
		]
	};
}
