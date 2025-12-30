using CleveCoding.Permissions.Entities;
using CleveCoding.Permissions.Models;
using CleveCoding.Permissions.Persistance;
using Microsoft.EntityFrameworkCore;

namespace CleveCoding.Permissions.Services;

public interface IUserDataAccessService
{
	/// <summary>
	/// Register data access for the given user and permission.
	/// </summary>
	/// <param name="userId"></param>
	/// <param name="permission"></param>
	/// <param name="cancellationToken"></param>
	/// <returns></returns>
	Task RegisterAsync(string userId, PermissionDescription permission, CancellationToken cancellationToken);

	/// <summary>
	/// Get data access logs for the given user within the specified date range.
	/// </summary>
	/// <param name="userId"></param>
	/// <param name="from"></param>
	/// <param name="to"></param>
	/// <returns></returns>
	Task<IEnumerable<UserDataAccessLog>?> GetDataAccessLogsAsync(string userId, DateTime from, DateTime to);

	/// <summary>
	/// Anonymize data access logs older than the specified date for the given data category.
	/// </summary>
	/// <param name="dataCategory"></param>
	/// <param name="date"></param>
	/// <returns></returns>
	Task AnonymizeOlderThanAsync(UserDataCategory dataCategory, DateTime date);

	/// <summary>
	/// Delete data access logs older than the specified date for the given data category.
	/// </summary>
	/// <param name="dataCategory"></param>
	/// <param name="date"></param>
	/// <returns></returns>
	Task DeleteOlderThanAsync(UserDataCategory dataCategory, DateTime date);
}

public class UserDataAccessService(PermissionDbContext Context, IUserAccessor UserAccessor)
	: IUserDataAccessService
{
	/// <inheritdoc/>
	public async Task RegisterAsync(string userId, PermissionDescription permission, CancellationToken cancellationToken)
	{
		var accessTime = DateTime.UtcNow;
		var groupId = Guid.NewGuid();
		foreach (var category in permission.DataCategories)
		{
			await Context.UserDataAccessLogs.AddAsync(new UserDataAccessLogEntity
			{
				UserId = userId,
				AccessedByUserId = UserAccessor.CurrentUser!.AccountName,
				Action = permission.Action,
				DataCategory = category,
				AccessGroupId = groupId,
				CreatedAt = accessTime
			}, cancellationToken);
		}

		await Context.SaveChangesAsync(cancellationToken);
	}

	/// <inheritdoc/>
	public async Task<IEnumerable<UserDataAccessLog>?> GetDataAccessLogsAsync(string userId, DateTime from, DateTime to)
	{
		return await Context.UserDataAccessLogs
			.Where(x => x.UserId == userId && x.CreatedAt.Date >= from.Date && x.CreatedAt.Date <= to.Date)
			.OrderByDescending(x => x.CreatedAt)
			.AsNoTracking()
			.Select(x => new UserDataAccessLog
			{
				Id = x.Id,
				UserId = x.UserId,
				AccessedByUserId = x.AccessedByUserId,
				AccessGroupId = x.AccessGroupId,
				Action = x.Action,
				DataCategory = x.DataCategory,
				CreatedAt = x.CreatedAt
			})
			.ToListAsync();
	}

	/// <inheritdoc/>
	public Task AnonymizeOlderThanAsync(UserDataCategory dataCategory, DateTime date)
	{
		throw new NotImplementedException();
	}

	/// <inheritdoc/>
	public Task DeleteOlderThanAsync(UserDataCategory dataCategory, DateTime date)
	{
		throw new NotImplementedException();
	}
}
