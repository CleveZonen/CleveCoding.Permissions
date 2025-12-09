using CleveCoding.Kernel;
using CleveCoding.Permissions.Exceptions;
using MediatR.Pipeline;
using Microsoft.AspNetCore.Http;
using NLog;

namespace CleveCoding.Permissions.Middleware;

/// <summary>
/// Used in the Middleware to catch any thrown ForbiddenException.
/// </summary>
public class ForbiddenExceptionHandler : IMiddleware
{
	public async Task InvokeAsync(HttpContext context, RequestDelegate next)
	{
		try
		{
			await next(context);
		}
		catch (ForbiddenException ex)
		{
			context.Response.StatusCode = StatusCodes.Status403Forbidden;
			await context.Response.WriteAsync(ex.Message);
		}
	}
}

/// <summary>
/// Used by MediatR to catch any thrown ForbiddenException. 
/// </summary>
/// <typeparam name="TRequest"></typeparam>
/// <typeparam name="TResponse"></typeparam>
/// <typeparam name="TException"></typeparam>
public class ForbiddenExceptionHandler<TRequest, TResponse, TException>
	: IRequestExceptionHandler<TRequest, TResponse, TException>
		where TResponse : Result
		where TRequest : IFeatureCommand<TResponse>
		where TException : ForbiddenException
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
