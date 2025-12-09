using CleveCoding.Kernel;
using CleveCoding.Permissions.Exceptions;
using MediatR;

namespace CleveCoding.Permissions.Behaviors;

/// <summary>
/// Behavior used by MediatR to check if every requests is permited.
/// 
/// @ForbiddenException
///		Gets handled by CleveCoding.Permissions.Middleware.ForbiddenExceptionHandler<TRequest, TResponse, TException>
/// </summary>
/// <typeparam name="TRequest"></typeparam>
/// <typeparam name="TResponse"></typeparam>
/// <param name="accessor"></param>
public class VerifyPermissionBehavior<TRequest, TResponse>(IUserAccessor accessor)
	: IPipelineBehavior<TRequest, TResponse>
		where TRequest : IRequirePermission
		where TResponse : Result
{
	public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
	{
		// get the user.
		var user = accessor.CurrentUser
			?? throw new ForbiddenException("Unauthorized user access - unkown user.");

		// skip check if the user is admin.
		if (accessor.IsAdmin(user))
		{
			return await next(cancellationToken);
		}

		// check the permission for the user.
		if (!user.HasPermission(request.RequiredPermission))
		{
			throw new ForbiddenException();
		}

		return await next(cancellationToken);
	}
}
