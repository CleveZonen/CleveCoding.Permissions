namespace CleveCoding.Permissions.Exceptions;

public class ForbiddenException : Exception
{
	public ForbiddenException() : base("You do not have permission to access the requested resource.") { }

	public ForbiddenException(string message) : base(message) { }

	public ForbiddenException(string message, Exception innerException)
		: base(message, innerException) { }
}
