using CleveCoding.Permissions.Services;
using Microsoft.AspNetCore.Http;

namespace CleveCoding.Permissions.Middleware;

/// <summary>
/// Load the user permissions.
/// </summary>
public class UserContextInitializer(IUserAccessor userAccessor, IPermissionService permissionService) : IMiddleware
{
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        var user = userAccessor.CurrentUser;

        if (user != null)
        {
            user.Permissions = await permissionService.GetUserPermissionsAsync(user);
        }

        await next(context);
    }
}
