namespace CleveCoding.Permissions;

/// <summary>
/// Defines a permission set with
/// the data categories coupled.
/// </summary>
public sealed class PermissionDescription
{
	public UserActionType ActionType { get; init; }
	public required string Resource { get; init; }
	public required string Description { get; init; }

	public IReadOnlyCollection<UserDataCategory> DataCategories { get; init; } = [];
	public bool ContainsPersonalData => DataCategories.Count > 0;
}