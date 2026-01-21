using CleveCoding.Kernel;
using CleveCoding.Permissions.Entities;
using CleveCoding.Permissions.Models;
using CleveCoding.Permissions.Persistance;
using Microsoft.EntityFrameworkCore;

namespace CleveCoding.Permissions.Services;

public interface IPermissionService
{
	/// <summary>
	/// Get the permissions for the given user, including role permissions.
	/// </summary>
	/// <param name="user"></param>
	/// <returns></returns>
	Task<IEnumerable<UserPermission>?> GetUserPermissionsAsync(IUserAccount user);

	/// <summary>
	/// Get the permissions for the given user.
	/// </summary>
	/// <param name="account"></param>
	/// <returns></returns>
	Task<IEnumerable<UserPermission>?> GetUserPermissionsAsync(string account);

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

	/// <summary>
	/// Get the permission audits for the given user.
	/// </summary>
	/// <param name="userId"></param>
	/// <returns></returns>
	Task<IEnumerable<UserPermissionAudit>?> GetAuditsForUserAsync(string userId);

	/// <summary>
	/// Get the permission audits for the given role.
	/// </summary>
	/// <param name="roleId"></param>
	/// <returns></returns>
	Task<IEnumerable<UserPermissionAudit>?> GetAuditsForRoleAsync(string roleId);
}

public class PermissionService(IDbContextFactory<PermissionDbContext> Factory, PermissionCache PermissionCache, IUserAccessor UserAccessor, IUserLookupService UserLookupService)
	: IPermissionService
{
	/// <inheritdoc/>
	public async Task<IEnumerable<UserPermission>?> GetUserPermissionsAsync(IUserAccount user)
	{
		var context = await Factory.CreateDbContextAsync();

		var cached = await PermissionCache.GetForUserAsync(user.AccountName);
		if (cached != null)
		{
			return cached;
		}

		user.Roles ??= [];

		var roles = user.Roles;
		var account = user.AccountName;

		var perms = await context.UserPermissions
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
	public async Task<IEnumerable<UserPermission>?> GetUserPermissionsAsync(string account)
	{
		var context = await Factory.CreateDbContextAsync();

		var perms = await context.UserPermissions
			.Where(p => p.UserId == account)
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

		var context = await Factory.CreateDbContextAsync();

		var perms = await context.UserPermissions
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

		var context = await Factory.CreateDbContextAsync();

		await using var transaction = await context.Database.BeginTransactionAsync();

		try
		{
			var existing = await context.UserPermissions.FirstOrDefaultAsync(x =>
			x.UserId == userId &&
			x.Resource == permission.Resource &&
			x.Action == permission.Action
		);

			// new permission.
			if (existing == null)
			{
				await context.UserPermissions.AddAsync(new UserPermissionEntity
				{
					UserId = userId,
					Resource = permission.Resource,
					Action = permission.Action,
					HasAccess = newValue,
					CreatedAt = now,
					CreatedBy = actor
				});
				await context.UserPermissionAudits.AddAsync(new UserPermissionAuditEntity
				{
					UserId = userId,
					Resource = permission.Resource,
					Action = permission.Action,
					OldValue = !newValue,
					CreatedAt = now,
					CreatedBy = actor
				});
			}
			// change permission.
			else
			{
				context.UserPermissions.Remove(existing);

				var updated = new UserPermissionEntity
				{
					UserId = userId,
					Resource = existing.Resource,
					Action = existing.Action,
					HasAccess = newValue,
					CreatedAt = now,
					CreatedBy = actor
				};

				await context.UserPermissions.AddAsync(updated);
				await context.UserPermissionAudits.AddAsync(new UserPermissionAuditEntity
				{
					UserId = userId,
					Resource = updated.Resource,
					Action = updated.Action,
					OldValue = existing.HasAccess,
					CreatedAt = now,
					CreatedBy = actor
				});
			}

			await context.SaveChangesAsync();
			await transaction.CommitAsync();
		}
		catch
		{
			await transaction.RollbackAsync();
			throw;
		}

		// invalidate cache
		await PermissionCache.InvalidateForUserAsync(userId);
	}

	/// <inheritdoc/>
	public async Task SetRolePermissionsAsync(UserPermission permission, bool newValue)
	{
		var now = DateTime.UtcNow;
		var actor = UserAccessor.CurrentUser!.AccountName;
		var roleId = permission.RoleId
			?? throw new ArgumentException("RoleId cannot be null for role permissions.");

		var context = await Factory.CreateDbContextAsync();

		await using var transaction = await context.Database.BeginTransactionAsync();

		try
		{
			var existing = await context.UserPermissions.FirstOrDefaultAsync(x =>
			x.UserId == null &&
			x.RoleId == roleId &&
			x.Resource == permission.Resource &&
			x.Action == permission.Action
		);

			// new permission.
			if (existing == null)
			{
				await context.UserPermissions.AddAsync(new UserPermissionEntity
				{
					RoleId = roleId,
					Resource = permission.Resource,
					Action = permission.Action,
					HasAccess = newValue,
					CreatedAt = now,
					CreatedBy = actor
				});
				await context.UserPermissionAudits.AddAsync(new UserPermissionAuditEntity
				{
					RoleId = roleId,
					Resource = permission.Resource,
					Action = permission.Action,
					OldValue = false,
					CreatedAt = now,
					CreatedBy = actor
				});

				await context.SaveChangesAsync();
				await transaction.CommitAsync();
			}
			// change permission.
			else if (existing.HasAccess != newValue)
			{
				context.UserPermissions.Remove(existing);

				var updated = new UserPermissionEntity
				{
					RoleId = existing.RoleId,
					Resource = existing.Resource,
					Action = existing.Action,
					HasAccess = newValue,
					CreatedAt = now,
					CreatedBy = actor
				};

				await context.UserPermissions.AddAsync(updated);
				await context.UserPermissionAudits.AddAsync(new UserPermissionAuditEntity
				{
					RoleId = roleId,
					Resource = updated.Resource,
					Action = updated.Action,
					OldValue = existing.HasAccess,
					CreatedAt = now,
					CreatedBy = actor
				});

				await context.SaveChangesAsync();
				await transaction.CommitAsync();
			}
		}
		catch
		{
			await transaction.RollbackAsync();
			throw;
		}

		// invalidate caches
		await PermissionCache.InvalidateForRoleAsync(permission.RoleId);
		foreach (var user in UserLookupService.GetUsersInRole(roleId) ?? [])
		{
			await PermissionCache.InvalidateForUserAsync(user.SamAccountName);
		}
	}

	/// <inheritdoc/>
	public async Task<IEnumerable<UserPermissionAudit>?> GetAuditsForUserAsync(string userId)
	{
		var context = await Factory.CreateDbContextAsync();

		return await context.UserPermissionAudits
			.Where(x => x.UserId != null && x.UserId == userId)
			.OrderByDescending(x => x.CreatedAt)
			.Take(1000)
			.AsNoTracking()
			.Select(x => new UserPermissionAudit
			{
				UserId = x.UserId,
				RoleId = x.RoleId,
				Resource = x.Resource,
				Action = x.Action,
				OldValue = x.OldValue,
				CreatedAt = x.CreatedAt,
				CreatedBy = x.CreatedBy,
			})
			.ToListAsync();
	}

	/// <inheritdoc/>
	public async Task<IEnumerable<UserPermissionAudit>?> GetAuditsForRoleAsync(string roleId)
	{
		var context = await Factory.CreateDbContextAsync();

		return await context.UserPermissionAudits
			.Where(x => x.RoleId == roleId && x.UserId == null)
			.OrderByDescending(x => x.CreatedAt)
			.Take(1000)
			.AsNoTracking()
			.Select(x => new UserPermissionAudit
			{
				RoleId = x.RoleId,
				Resource = x.Resource,
				Action = x.Action,
				OldValue = x.OldValue,
				CreatedAt = x.CreatedAt,
				CreatedBy = x.CreatedBy,
			})
			.ToListAsync();
	}
}
