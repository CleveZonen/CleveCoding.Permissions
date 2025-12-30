using CleveCoding.Kernel;
using CleveCoding.Permissions.Entities;
using CleveCoding.Permissions.Persistance;
using MediatR;

namespace CleveCoding.Permissions.Behaviors;

public sealed class UserDataAccessLogBehaviour<TRequest, TResponse>(IUserAccessor UserAccessor, PermissionDbContext permissionDbContext)
	: IPipelineBehavior<TRequest, TResponse>
		where TRequest : IRequest<TResponse>
		where TResponse : Result
{
	public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
	{
		var response = await next(cancellationToken);

		if (request is not IRequirePermission permissionRequest)
			return response;

		var permission = permissionRequest.RequiredPermission;

		if (!permission.ContainsPersonalData ||
			permission.Action is not UserActionType.ViewDetails
									 and not UserActionType.ViewIndex
									 and not UserActionType.Export
									 and not UserActionType.Download)
		{
			return response;
		}

		if (request is not IUserDataAccessScopedRequest employeeRequest)
			return response;

		var accessTime = DateTime.UtcNow;
		var groupId = Guid.NewGuid();
		foreach (var category in permission.DataCategories)
		{
			await permissionDbContext.UserDataAccessLogs.AddAsync(new UserDataAccessLogEntity
			{
				UserId = employeeRequest.UserId,
				AccessedByUserId = UserAccessor.CurrentUser!.AccountName,
				Action = permission.Action,
				DataCategory = category,
				AccessGroupId = groupId,
				CreatedAt = accessTime
			}, cancellationToken);
		}

		await permissionDbContext.SaveChangesAsync(cancellationToken);

		return response;
	}
}
