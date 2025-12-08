using CleveCoding.Permissions;
using CleveCoding.Permissions.Models;

namespace CleveCoding.PermissionsApp.Features.UserReviews;

/// <summary>
/// Example Request implementing IRequirePermission.
/// </summary>
public class GetUserReviewsForIndexRequest : IRequirePermission
{
	public PermissionDescription RequiredPermission => new(nameof(UserReviews), UserActionType.ViewIndex, "Access to user reviews index.");
}
