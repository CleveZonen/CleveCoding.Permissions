#region

using FluentValidation;

#endregion

namespace CleveCoding.Kernel.Extensions;

public static class IValidatorExtensions
{
	/// <summary>
	///     Validate the <paramref name="model" /> with FluentValidation.
	///     Sets the response on failure when validation failed, else success.
	/// </summary>
	/// <typeparam name="TModel"></typeparam>
	/// <typeparam name="TReponse"></typeparam>
	/// <typeparam name="TResult"></typeparam>
	/// <param name="validator"></param>
	/// <param name="model"></param>
	/// <param name="response"></param>
	/// <param name="cancellationToken"></param>
	/// <returns></returns>
	public static async Task<TReponse> ValidateAsync<TModel, TReponse, TResult>(this IValidator<TModel> validator,
		TModel model,
		TReponse response,
		CancellationToken cancellationToken = default)
		where TReponse : Result<TResult>
		where TModel : class
	{
		var validationResult = await validator.ValidateAsync(model, cancellationToken);
		if (validationResult is null || validationResult.IsValid)
		{
			response.SetSuccess();
			return response;
		}

		response.SetFailure(validationResult);

		return response;
	}
}