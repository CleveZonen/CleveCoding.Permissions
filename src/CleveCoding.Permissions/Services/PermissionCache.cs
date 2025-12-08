using CleveCoding.Permissions.Models;
using Microsoft.Extensions.Caching.Memory;

namespace CleveCoding.Permissions.Services;

public class PermissionCache(IMemoryCache Cache)
{
    public Task<IEnumerable<UserPermission>?> GetForUserAsync(string userId)
    {
        return Task.FromResult(Cache.Get<IEnumerable<UserPermission>>($"permissions_user_{userId}"));
    }

    public Task SetForUserAsync(string userId, IEnumerable<UserPermission> permissions)
    {
        Cache.Set($"permissions_user_{userId}", permissions, TimeSpan.FromHours(12));
        return Task.CompletedTask;
    }

    public Task InvalidateForUserAsync(string userId)
    {
        Cache.Remove($"permissions_user_{userId}");
        return Task.CompletedTask;
    }

    public Task<IEnumerable<UserPermission>?> GetForRoleAsync(string roleId)
    {
        return Task.FromResult(Cache.Get<IEnumerable<UserPermission>>($"permissions_role_{roleId}"));
    }

    public Task SetForRoleAsync(string roleId, IEnumerable<UserPermission> permissions)
    {
        Cache.Set($"permissions_role_{roleId}", permissions, TimeSpan.FromHours(12));
        return Task.CompletedTask;
    }

    public Task InvalidateForRoleAsync(string userId)
    {
        Cache.Remove($"permissions_role_{userId}");
        return Task.CompletedTask;
    }
}
