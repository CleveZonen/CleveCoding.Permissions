#region

using MediatR;

#endregion

namespace CleveCoding.Kernel;

/// <summary>
///     ICommand interface for exposing a NewResponse-method to build a appropriate Response.
/// </summary>
public interface ICommand : ICommand<Result>
{
}

/// <summary>
///     ICommand interface for exposing a generic NewResponse-method to build a appropriate Response.
/// </summary>
public interface ICommand<out TResponse> : IRequest<TResponse>
{
    TResponse NewResponse();
}