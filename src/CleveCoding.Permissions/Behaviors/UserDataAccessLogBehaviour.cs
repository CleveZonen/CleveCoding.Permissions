using CleveCoding.Kernel;
using CleveCoding.Permissions.Services;
using MediatR;

namespace CleveCoding.Permissions.Behaviors;

public sealed class UserDataAccessLogBehaviour<TRequest, TResponse>(IUserDataAccessService userDataAccessService)
	: IPipelineBehavior<TRequest, TResponse>
		where TRequest : IRequest<TResponse>
		where TResponse : Result
{
	public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
	{
		var response = await next(cancellationToken);

		if (request is not IRequirePermission permissionRequest)
			return response;

		// Only log if the permission involves personal data and the action is relevant.
		var permission = permissionRequest.RequiredPermission;
		if (!permission.ContainsPersonalData ||
			permission.Action is not UserActionType.ViewDetails
								and not UserActionType.ViewIndex
								and not UserActionType.Export
								and not UserActionType.Download)
		{
			return response;
		}

		// Ensure the request contains user context.
		if (request is not IUserDataAccessScopedRequest employeeRequest)
			return response;

		// Register the data access.
		await userDataAccessService.RegisterAsync(employeeRequest.UserId, permission, cancellationToken);

		return response;
	}
}
