namespace CleveCoding.Kernel;

public interface IUserAccount
{
	int Id { get; set; }
	string AccountName { get; set; }
	string? EmailAddress { get; set; }
	IEnumerable<string>? Roles { get; set; }

	bool IsInRoles(params string[] roles);
}