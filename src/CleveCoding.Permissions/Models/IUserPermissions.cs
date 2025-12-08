namespace CleveCoding.Permissions.Models;

public interface IUserPermissions
{
    // mixed user- and role permissions applicable for current user.
    IEnumerable<UserPermission>? Permissions { get; set; }
}
