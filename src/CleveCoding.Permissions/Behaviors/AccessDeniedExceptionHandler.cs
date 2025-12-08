using CleveCoding.Kernel;
using MediatR.Pipeline;
using NLog;

namespace CleveCoding.Permissions.Behaviors;

public class AccessDeniedExceptionHandler<TRequest, TResponse, TException>
    : IRequestExceptionHandler<TRequest, TResponse, TException>
        where TResponse : Result
        where TRequest : IFeatureCommand<TResponse>
        where TException : AccessDeniedException
{
    private readonly Logger _logger = LogManager.GetCurrentClassLogger();

    /// <inheritdoc />
    public Task Handle(TRequest request, TException exception, RequestExceptionHandlerState<TResponse> state, CancellationToken cancellationToken)
    {
        var response = request.NewResponse();

        response.SetFailure(exception.Message);

        state.SetHandled(response);
        _logger.Fatal(exception);

        return Task.CompletedTask;
    }
}