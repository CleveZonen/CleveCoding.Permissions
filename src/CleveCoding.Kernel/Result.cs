#region

using System.Net;
using CleveCoding.Kernel.Extensions.Hosting;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http;


#endregion

namespace CleveCoding.Kernel;

/// <summary>
///     Result interface with an covariance generic parameter for the Data.
/// </summary>
/// <typeparam name="TData"></typeparam>
public interface IResult<out TData>
{
	TData? Data { get; }
}

/// <summary>
///     Simple Result record for containing information about a request and containing possible Data.
/// </summary>
/// <typeparam name="TData"></typeparam>
public record Result<TData> : Result, IResult<TData>
{
	public TData? Data { get; set; }

	public void SetSuccess(TData? data)
	{
		IsSuccess = true;
		Data = data;
	}
}

/// <summary>
///     Simple Result record for containing information about a request.
/// </summary>
public record Result
{
	public bool IsSuccess { get; set; }

	public bool HasError { get; set; }

	public int? ErrorCode { get; set; }

	public IEnumerable<string>? ErrorMessages { get; set; }

	public DateTime ServerExecutionStartTime { get; set; }

	public TimeSpan ServerExecutionDuration { get; set; }

	public void SetSuccess()
	{
		IsSuccess = true;
	}

	public void SetFailure(ValidationResult result)
	{
		SetFailure(result.Errors.Select(x => x.ErrorMessage), (int)HttpStatusCode.BadRequest);
	}

	public void SetFailure(Exception exception)
	{
		var errorMessage = CleveEnvironments.IsProduction
			? "An technical error has occured."
			: exception.Message;

		SetFailure(errorMessage, StatusCodes.Status500InternalServerError);
	}

	public void SetFailure(IEnumerable<string> errors, int? errorCode = null)
	{
		IsSuccess = false;
		HasError = true;
		ErrorMessages = errors;
		ErrorCode = errorCode;
	}

	public void SetFailure(string errorMessage, int? errorCode = null)
	{
		IsSuccess = false;
		HasError = true;
		ErrorMessages = [errorMessage];
		ErrorCode = errorCode;
	}
}
