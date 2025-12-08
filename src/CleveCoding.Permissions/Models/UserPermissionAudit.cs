namespace CleveCoding.Permissions.Models;

public record UserPermissionAudit
{
    public string? UserId { get; init; }
    public string? RoleId { get; init; }

    public string Resource { get; init; } = null!;
    public UserActionType Action { get; init; }
    public bool NewValue { get; init; }

    /// <summary>
    /// DateTime the change has occured.
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// User that triggered the audit.
    /// </summary>
    public string CreatedBy { get; set; } = null!;
}
