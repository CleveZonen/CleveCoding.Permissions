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
public class VerifyPermissionBehavior<TRequest, TResponse>(IPermissionEvaluator permissionEvaluator)
	: IPipelineBehavior<TRequest, TResponse>
		where TRequest : IRequirePermission
		where TResponse : Result
{
	public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
	{
		var permission = TRequest.RequiredPermission;
		if (!permissionEvaluator.HasPermission(permission))
		{
			throw new ForbiddenException
			{
				Resource = permission.Resource,
				Action = permission.Action
			};
		}

		return await next(cancellationToken);
	}
}
