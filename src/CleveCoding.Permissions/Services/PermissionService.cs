using CleveCoding.Kernel;
using CleveCoding.Permissions.Entities;
using CleveCoding.Permissions.Models;
using CleveCoding.Permissions.Persistance;
using Microsoft.EntityFrameworkCore;

namespace CleveCoding.Permissions.Services;

public interface IPermissionService
{
    /// <summary>
    /// Get the permissions for the given user.
    /// </summary>
    /// <param name="user"></param>
    /// <returns></returns>
    Task<IEnumerable<UserPermission>?> GetUserPermissionsAsync(IUserAccount user);

    /// <summary>
    /// Get the permissions for the given role.
    /// </summary>
    /// <param name="roleId"></param>
    /// <returns></returns>
    Task<IEnumerable<UserPermission>?> GetRolePermissionsAsync(string roleId);

    /// <summary>
    /// Update the permission for an user.
    /// </summary>
    /// <param name="permission"></param>
    /// <param name="newValue"></param>
    /// <returns></returns>
    Task SetUserPermissionsAsync(UserPermission permission, bool newValue);

    /// <summary>
    /// Update the permission for an role.
    /// </summary>
    /// <param name="permission"></param>
    /// <param name="newValue"></param>
    /// <returns></returns>
    Task SetRolePermissionsAsync(UserPermission permission, bool newValue);

    // todo: get audits for user
    // todo: get audits for role
}

public class PermissionService(PermissionDbContext Context, PermissionCache PermissionCache, IUserAccessor UserAccessor) : IPermissionService
{
    /// <inheritdoc/>
    public async Task<IEnumerable<UserPermission>?> GetUserPermissionsAsync(IUserAccount user)
    {
        var cached = await PermissionCache.GetForUserAsync(user.AccountName);
        if (cached != null)
        {
            return cached;
        }

        user.Roles ??= [];

        var roles = user.Roles;
        var account = user.AccountName;

        var perms = await Context.UserPermissions
            .Where(p => p.UserId == account || (roles.Count() > 0 && roles.Contains(p.RoleId)))
            .ToListAsync();

        var effective = perms
            .OrderByDescending(p => p.UserId != null) // user overrides role
            .GroupBy(p => new { p.Resource, p.Action })
            .Select(g => g.First())
            .Select(x => new UserPermission
            {
                UserId = x.UserId,
                RoleId = x.RoleId,
                Action = x.Action,
                Resource = x.Resource,
                HasAccess = x.HasAccess,
                CreatedAt = x.CreatedAt,
                CreatedBy = x.CreatedBy
            });

        await PermissionCache.SetForUserAsync(user.AccountName, effective);

        return effective;
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<UserPermission>?> GetRolePermissionsAsync(string roleId)
    {
        if (string.IsNullOrWhiteSpace(roleId))
        {
            return [];
        }

        var cached = await PermissionCache.GetForRoleAsync(roleId);
        if (cached != null)
        {
            return cached;
        }

        var perms = await Context.UserPermissions
            .Where(p => p.RoleId == roleId)
            .ToListAsync();

        var effective = perms
            .GroupBy(p => new { p.Resource, p.Action })
            .Select(g => g.First())
            .Select(x => new UserPermission
            {
                UserId = x.UserId,
                RoleId = x.RoleId,
                Action = x.Action,
                Resource = x.Resource,
                HasAccess = x.HasAccess,
                CreatedAt = x.CreatedAt,
                CreatedBy = x.CreatedBy
            });

        await PermissionCache.SetForRoleAsync(roleId, effective);

        return effective;
    }

    /// <inheritdoc/>
    public async Task SetUserPermissionsAsync(UserPermission permission, bool newValue)
    {
        var now = DateTime.UtcNow;
        var actor = UserAccessor.CurrentUser!.AccountName;
        var userId = permission.UserId
            ?? throw new ArgumentException("UserId cannot be null for user permissions.");
        var existing = await Context.UserPermissions.FirstOrDefaultAsync(x =>
            x.UserId == userId &&
            x.Resource == permission.Resource &&
            x.Action == permission.Action
        );

        // create permission.
        if (existing == null)
        {
            await Context.UserPermissions.AddAsync(new UserPermissionEntity
            {
                UserId = userId,
                Resource = permission.Resource,
                Action = permission.Action,
                HasAccess = newValue,
                CreatedAt = now,
                CreatedBy = actor
            });
            await Context.UserPermissionAudits.AddAsync(new UserPermissionAuditEntity
            {
                UserId = userId,
                Resource = permission.Resource,
                Action = permission.Action,
                OldValue = false,
                CreatedAt = now,
                CreatedBy = actor
            });

            await Context.SaveChangesAsync();
            await PermissionCache.InvalidateForUserAsync(userId);

            return;
        }

        // update permission.
        if (existing.HasAccess != newValue)
        {
            Context.UserPermissions.Remove(existing);

            var updated = new UserPermissionEntity
            {
                UserId = userId,
                Resource = existing.Resource,
                Action = existing.Action,
                HasAccess = newValue,
                CreatedAt = now,
                CreatedBy = actor
            };

            await Context.UserPermissions.AddAsync(updated);
            await Context.UserPermissionAudits.AddAsync(new UserPermissionAuditEntity
            {
                UserId = userId,
                Resource = updated.Resource,
                Action = updated.Action,
                OldValue = existing.HasAccess,
                CreatedAt = now,
                CreatedBy = actor
            });

            await Context.SaveChangesAsync();
            await PermissionCache.InvalidateForUserAsync(userId);
        }
    }

    /// <inheritdoc/>
    public async Task SetRolePermissionsAsync(UserPermission permission, bool newValue)
    {
        var now = DateTime.UtcNow;
        var actor = UserAccessor.CurrentUser!.AccountName;
        var roleId = permission.RoleId
            ?? throw new ArgumentException("RoleId cannot be null for role permissions.");
        var existing = await Context.UserPermissions.FirstOrDefaultAsync(x =>
            x.UserId == null &&
            x.RoleId == roleId &&
            x.Resource == permission.Resource &&
            x.Action == permission.Action
        );

        if (existing == null)
        {
            await Context.UserPermissions.AddAsync(new UserPermissionEntity
            {
                RoleId = roleId,
                Resource = permission.Resource,
                Action = permission.Action,
                HasAccess = newValue,
                CreatedAt = now,
                CreatedBy = actor
            });
            await Context.UserPermissionAudits.AddAsync(new UserPermissionAuditEntity
            {
                RoleId = roleId,
                Resource = permission.Resource,
                Action = permission.Action,
                OldValue = false,
                CreatedAt = now,
                CreatedBy = actor
            });

            await Context.SaveChangesAsync();

            // invalidate caches
            await PermissionCache.InvalidateForRoleAsync(permission.RoleId);
            await InvalidateUserCachesForRoleAsync(roleId);

            return;
        }

        if (existing.HasAccess != newValue)
        {
            Context.UserPermissions.Remove(existing);

            var updated = new UserPermissionEntity
            {
                RoleId = existing.RoleId,
                Resource = existing.Resource,
                Action = existing.Action,
                HasAccess = newValue,
                CreatedAt = now,
                CreatedBy = actor
            };

            await Context.UserPermissions.AddAsync(updated);
            await Context.UserPermissionAudits.AddAsync(new UserPermissionAuditEntity
            {
                RoleId = roleId,
                Resource = updated.Resource,
                Action = updated.Action,
                OldValue = existing.HasAccess,
                CreatedAt = now,
                CreatedBy = actor
            });

            await Context.SaveChangesAsync();
            await PermissionCache.InvalidateForRoleAsync(permission.RoleId);
            await InvalidateUserCachesForRoleAsync(roleId);
        }
    }
}
