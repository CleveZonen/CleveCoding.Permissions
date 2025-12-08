using CleveCoding.Kernel;
using CleveCoding.Permissions.Extensions;
using MediatR;

namespace CleveCoding.Permissions.Behaviors;

public class VerifyPermissionBehavior<TRequest, TResponse>(IUserAccessor accessor)
    : IPipelineBehavior<TRequest, TResponse>
        where TRequest : IRequirePermission
        where TResponse : Result
{
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var user = accessor.CurrentUser
            ?? throw new AccessDeniedException("Unauthorized user access.");

        if (!user.HasPermission(request.RequiredPermission))
        {
            throw new AccessDeniedException($"Access Denied to {request.RequiredPermission.Action} {request.RequiredPermission.Resource}.");
        }

        return await next(cancellationToken);
    }
}
