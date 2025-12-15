using CleveCoding.Kernel.Extensions;
using CleveCoding.Permissions.Models;

namespace CleveCoding.Permissions.UI;

/// <summary>
/// Helper class to organize and group the permissions
/// based on the resource.
/// </summary>
public record PermissionSet
{
	public required string Resource { get; set; }

	public required IEnumerable<UserPermission> Permissions { get; set; } = null!;

	public UserPermission? Get(UserActionType action)
		=> Permissions.FirstOrDefault(x => x.Action == action);

	public PermissionSet CopyFor(string groupName) => new()
	{
		Resource = Resource,
		Permissions = Permissions.Where(x => x.Action.GetGroup()?.Equals(groupName, StringComparison.CurrentCultureIgnoreCase) == true)
	};

}
