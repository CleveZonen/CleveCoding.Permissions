namespace CleveCoding.Permissions;

/// <summary>
/// Defines a permission set with
/// the data categories coupled.
/// </summary>
/// <param name="Resource"></param>
/// <param name="Action"></param>
/// <param name="Description"></param>
public sealed class PermissionDescription(string Resource, UserActionType Action, string Description)
{
	/// <summary>
	/// The User Data Categories associated with this permission.
	/// </summary>
	public IReadOnlyCollection<UserDataCategory> DataCategories { get; init; } = [];

	/// <summary>
	/// Gets a value indicating whether the data contains any personal information categories.
	/// </summary>
	public bool ContainsPersonalData => DataCategories.Count > 0;
}