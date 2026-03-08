namespace CleveCoding.Permissions;

/// <summary>
/// Defines a permission set with
/// the data categories coupled.
/// </summary>
public sealed class PermissionDescription
{
	/// <summary>
	/// Indicates the type of action performed on the user data.
	/// </summary>
	public UserActionType Action { get; set; }

	/// <summary>
	/// Optional unique identifier for the action, which can be used for more granular permission checks.
	/// </summary>
	public string? ActionId { get; set; }

	/// <summary>
	/// Gets the the name of the resource the permission is associated with.
	/// </summary>
	public string Resource { get; set; } = null!;

	/// <summary>
	/// Description of the permission.
	/// </summary>
	public string Description { get; set; } = null!;

	/// <summary>
	/// Permission only when the user is an admin.
	/// </summary>
	public bool AdminAccessOnly { get; set; }

	/// <summary>
	/// The User Data Categories associated with this permission.
	/// </summary>
	public IReadOnlyCollection<UserDataCategory> DataCategories { get; init; } = [];

	/// <summary>
	/// Gets a value indicating whether the data contains any personal information categories.
	/// </summary>
	public bool ContainsPersonalData => DataCategories.Count > 0;
}