namespace CleveCoding.Permissions.Models;

/// <summary>
/// Defines a permission set existing
/// out of the resource and action.
/// </summary>
/// <param name="Resource"></param>
/// <param name="Action"></param>
/// <param name="Description"></param>
public record PermissionDescription(string Resource, UserActionType Action, string? Description = null);
