namespace CleveCoding.Permissions.Models;

public record UserDataAccessLog
{
	/// <summary>
	/// Unique identifier for the access log entry.
	/// </summary>
	public long Id { get; set; }

	/// <summary>
	/// Gets or sets the ID of the user whose data was accessed.
	/// </summary>
	public required string UserId { get; set; }

	/// <summary>
	/// Gets or sets the ID of the user who accessed the data.
	/// </summary>
	public required string AccessedByUserId { get; set; }

	/// <summary>
	/// Unique identifier for grouping multiple access with an single request.
	/// AccessLogs with the same AccessGroupId belong to the same request.
	/// </summary>
	public Guid AccessGroupId { get; set; }

	/// <summary>
	/// Indicates the type of action performed on the user data.
	/// </summary>
	public UserActionType Action { get; set; }

	/// <summary>
	/// Indicates the category of user data that was accessed.
	/// </summary>
	public UserDataCategory DataCategory { get; set; }

	/// <summary>
	/// DateTime when the access occurred.
	/// </summary>
	public DateTime CreatedAt { get; set; }
}
