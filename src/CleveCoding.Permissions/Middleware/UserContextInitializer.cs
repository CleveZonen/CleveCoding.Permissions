using CleveCoding.Permissions.Services;
using Microsoft.AspNetCore.Http;

namespace CleveCoding.Permissions.Middleware;

/// <summary>
/// Load the user context including permissions.
/// </summary>
public class UserContextInitializer(
	IUserAccessor userAccessor,
	IPermissionService permissionService
	) : IMiddleware
{
	/// <summary>
	/// Every HTTP request passing through the pipeline (middleware)
	/// will automatically load the user and their permissions.
	/// </summary>
	/// <param name="context"></param>
	/// <param name="next"></param>
	/// <returns></returns>
	public async Task InvokeAsync(HttpContext context, RequestDelegate next)
	{
		await InitializeAsync();

		await next(context);
	}

	/// <summary>
	/// Initialize the user with their permissions.
	/// </summary>
	/// <param name="forceReload"></param>
	/// <returns></returns>
	public async Task InitializeAsync(bool forceReload = false)
	{
		if (userAccessor.CurrentUser != null && !forceReload) return;

		await userAccessor.TryLoadUserAsync(forceReload, true);

		if (userAccessor.CurrentUser is { } user)
		{
			user.Permissions ??= await permissionService.GetUserPermissionsAsync(user);
		}
	}
}
