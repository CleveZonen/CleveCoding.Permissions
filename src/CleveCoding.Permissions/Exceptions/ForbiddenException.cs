using CleveCoding.Permissions.Models;

namespace CleveCoding.Permissions.Exceptions;

public class ForbiddenException : Exception
{
	public const string DefaultErrorMessage = "You do not have permission to access the requested resource.";

	public string? Resource { get; set; }
	public UserActionType? Action { get; set; }

	public ForbiddenException() : base(DefaultErrorMessage) { }

	public ForbiddenException(string message) : base(message) { }

	public ForbiddenException(string message, Exception innerException)
		: base(message, innerException) { }
}
