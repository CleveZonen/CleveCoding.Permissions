namespace CleveCoding.Permissions;

/// <summary>
/// Marks requests that access user-specific data.
/// IUserDataAccessScopedRequest used to log which user's data is being accessed.
/// </summary>
public interface IUserDataAccessScopedRequest
{
	/// <summary>
	/// Unique identifier of the user who's data is being accessed.
	/// </summary>
	string UserId { get; }
}