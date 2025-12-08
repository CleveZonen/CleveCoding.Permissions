using CleveCoding.Kernel.Entities;
using CleveCoding.Permissions.Models;

namespace CleveCoding.Permissions.Entities;

public record UserPermissionEntity : CreationEntity<int>
{
    public string? UserId { get; init; }
    public string? RoleId { get; init; }

    /// <summary>
    /// Should be exact match with IRequirePermission.PermissionDescription.Resource
    /// </summary>
    public string Resource { get; init; } = null!;
    public UserActionType Action { get; init; }
    public bool HasAccess { get; init; }
}
