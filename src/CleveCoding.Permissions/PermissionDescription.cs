namespace CleveCoding.Permissions;

/// <summary>
/// Defines a permission set with
/// the data categories coupled.
/// </summary>
/// <param name="Resource"></param>
/// <param name="Action"></param>
/// <param name="Description"></param>
public sealed class PermissionDescription
{
	/// <summary>
	/// Indicates the type of action performed on the user data.
	/// </summary>
	public UserActionType Action { get; set; }

	/// <summary>
	/// Gets the the name of the resource the permission is associated with.
	/// </summary>
	public required string Resource { get; set; }

	/// <summary>
	/// Description of the permission.
	/// </summary>
	public required string Description { get; set; }

	/// <summary>
	/// The User Data Categories associated with this permission.
	/// </summary>
	public IReadOnlyCollection<UserDataCategory> DataCategories { get; init; } = [];

	/// <summary>
	/// Gets a value indicating whether the data contains any personal information categories.
	/// </summary>
	public bool ContainsPersonalData => DataCategories.Count > 0;
}