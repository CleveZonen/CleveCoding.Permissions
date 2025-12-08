namespace CleveCoding.Permissions.Models;

public record UserPermission
{
    public string? UserId { get; init; }
    public string? RoleId { get; init; }
    public DateTime CreatedAt { get; set; }
    public string CreatedBy { get; set; } = null!;

    /// <summary>
    /// Should be exact match with IRequirePermission.PermissionDescription.Resource
    /// </summary>
    public string Resource { get; init; } = null!;
    public UserActionType Action { get; init; }
    public bool HasAccess { get; init; }
}
