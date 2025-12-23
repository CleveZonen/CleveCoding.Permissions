namespace CleveCoding.Permissions.Configurations;

public record PermissionConfigurations
{
	/// <summary>
	/// Connection string to the database
	/// where the User Permissions should be persisted.
	/// </summary>
	public string ConnectionString { get; set; } = null!;

	/// <summary>
	/// List of the administrators roles.
	/// </summary>
	public IEnumerable<string> AdminRoles { get; set; } = [];

	/// <summary>
	/// List of all the roles. Including administrators roles.
	/// </summary>
	public IEnumerable<string> Roles { get; set; } = [];

	/// <summary>
	/// The error page to redirect.
	/// Default: /errors/forbidden
	/// </summary>
	public string ErrorPageUrl { get; set; } = "/errors/forbidden";
}
