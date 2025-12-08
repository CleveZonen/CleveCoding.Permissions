using CleveCoding.Permissions;
using CleveCoding.Permissions.Models;

namespace CleveCoding.PermissionsApp.Features.UserReviews;

public class GetUserReviewsForIndexRequest : IRequirePermission
{
    public PermissionDescription RequiredPermission => new(nameof(UserReviews), UserActionType.ReadIndex, "Access to user reviews index.");
}
