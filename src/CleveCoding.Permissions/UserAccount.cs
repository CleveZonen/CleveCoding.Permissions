using CleveCoding.Kernel;
using CleveCoding.Permissions.Models;

namespace CleveCoding.Permissions;

public record UserAccount : IUserAccount, IUserPermissions
{
    public int Id { get; set; }
    public string UserName { get; set; } = null!;
    public string? FirstName { get; set; }
    public string? MiddleName { get; set; }
    public string? LastName { get; set; }
    public string? Description { get; set; }
    public string? TelephoneNumber { get; set; }
    public bool IsActive { get; set; }
    public string AccountName { get; set; } = null!;
    public string? EmailAddress { get; set; }

    public IEnumerable<string>? Roles { get; set; }
    public IEnumerable<UserPermission>? Permissions { get; set; }

    public bool IsInRoles(params string[] roles)
    {
        return Roles is not null && Roles.Any() && roles.Any(role => Roles.Contains(role));
    }

    public bool HasPermission(PermissionDescription permissionDescription)
    {
        return HasPermission(permissionDescription.Resource, permissionDescription.Action);
    }

    public bool HasPermission(string resource, UserActionType action)
    {
        return Permissions?.Any(p => p.Resource == resource && p.Action == action && p.HasAccess) ?? false;
    }
}
