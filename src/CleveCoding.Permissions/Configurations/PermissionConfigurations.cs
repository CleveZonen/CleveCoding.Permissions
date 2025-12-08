namespace CleveCoding.Permissions.Configurations;

public record PermissionConfigurations
{
    public string[] AdminRoles { get; set; } = [];
    public string ConnectionString { get; set; } = string.Empty;
}
