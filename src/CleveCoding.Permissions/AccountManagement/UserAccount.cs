using CleveCoding.Permissions.Models;

namespace CleveCoding.Kernel.AccountManagement;

// todo: this class is just here as placeholder.
// @implementation remove this class and reference the Application-project.
// @implementation update the UserAccount with IUserPermissions
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

    public IEnumerable<string>? HrmGroups { get; set; }
    public IEnumerable<UserPermission>? Permissions { get; set; }

    public bool IsInRoles(params string[] roles)
    {
        return Roles is not null && Roles.Any() && roles.Any(role => Roles.Contains(role));
    }
}
