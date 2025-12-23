using CleveCoding.Kernel;
using CleveCoding.Permissions.Models;

namespace CleveCoding.Permissions;

public record UserAccount : IUserAccount, IUserPermissions
{
	public int Id { get; set; }
	public string AccountName { get; set; } = null!;
	public string UserName { get; set; } = null!;
	public string? FirstName { get; set; }
	public string? MiddleName { get; set; }
	public string? LastName { get; set; }
	public string? Description { get; set; }
	public string? TelephoneNumber { get; set; }
	public bool IsActive { get; set; }
	public string? EmailAddress { get; set; }

	/// <summary>
	/// The names of the roles the user is member of.
	/// </summary>
	public IEnumerable<string>? Roles { get; set; }

	/// <inheritdoc/>
	public IEnumerable<UserPermission>? Permissions { get; set; }

	/// <inheritdoc/>
	public bool IsInRoles(IEnumerable<string> roles)
		=> Roles is not null && Roles.Any() && roles.Any(x => Roles.Contains(x));

	/// <inheritdoc/>
	public bool IsInRoles(IEnumerable<UserRole> roles)
		=> Roles is not null && Roles.Any() && roles.Any(x => Roles.Contains(x.Id));

	/// <inheritdoc/>
	public bool HasPermission(string resource, UserActionType action)
		=> Permissions?.Any(p => p.Resource == resource && p.Action == action && p.HasAccess) ?? false;

	/// <inheritdoc/>
	public bool HasPermission(PermissionDescription permissionDescription)
		=> HasPermission(permissionDescription.Resource, permissionDescription.Action);
}
