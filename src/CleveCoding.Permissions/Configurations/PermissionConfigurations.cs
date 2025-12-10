namespace CleveCoding.Permissions.Configurations;

public record PermissionConfigurations
{
	public string ConnectionString { get; set; } = null!;
	public string? ErrorPageUrl { get; set; }
	public string[] AdminRoles { get; set; } = [];
	public string[] Roles { get; set; } = [];
}
