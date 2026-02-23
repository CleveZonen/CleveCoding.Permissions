namespace CleveCoding.Permissions;

/// <summary>
/// Marks responses that accessed user-specific data.
/// IUserDataAccessScopedResponse used to log which user's data is being accessed.
/// </summary>
public interface IUserDataAccessedResponse
{
	/// <summary>
	/// Unique identifier of the user who's data is being accessed.
	/// </summary>
	string UserId { get; }
}
