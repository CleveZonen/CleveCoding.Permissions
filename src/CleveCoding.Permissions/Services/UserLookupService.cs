using System.DirectoryServices.AccountManagement;
using System.Runtime.Versioning;

[assembly: SupportedOSPlatform("windows")]

namespace CleveCoding.Permissions.Services;

public interface IUserLookupService
{
	/// <summary>
	/// Get list of all the current registered users in Active Directory.
	/// </summary>
	/// <returns></returns>
	UserPrincipal? GetUser(string accountName);

	/// <summary>
	/// Get list of all the current registered users in Active Directory from an role.
	/// </summary>
	/// <param name="roleId"></param>
	/// <returns></returns>
	IList<UserPrincipal>? GetUsers();

	/// <summary>
	/// Get user information from Active Directory based on Principal.SamAccountName.
	/// </summary>
	/// <param name="accountName"></param>
	/// <returns></returns>
	IList<UserPrincipal>? GetUsersInRole(string roleId);
}

public class UserLookupService
	: IUserLookupService
{
	private readonly HashSet<string> RequiredOuValues = new(["Users"]);

	/// <inheritdoc/>
	public IList<UserPrincipal>? GetUsers()
	{
		return FindUsers(new PrincipalSearcher(new UserPrincipal(new(ContextType.Domain))
		{
			UserPrincipalName = "*@*",
			Enabled = true
		}));
	}

	/// <inheritdoc/>
	public IList<UserPrincipal>? GetUsersInRole(string roleId)
	{
		return FindUsers(new PrincipalSearcher(new UserPrincipal(new(ContextType.Domain))
		{
			UserPrincipalName = "*@*",
			Enabled = true
		}))?.Where(x => x.GetGroups().OfType<GroupPrincipal>().Any(g => g.Name == roleId))
			.ToList();
	}

	/// <inheritdoc/>
	public UserPrincipal? GetUser(string accountName)
	{
		return FindUsers(new PrincipalSearcher(new UserPrincipal(new(ContextType.Domain))
		{
			SamAccountName = accountName,
			Enabled = true
		}))?.FirstOrDefault();
	}

	/// <summary>
	/// Search for the users in the Principle Store (AD) based on the search params.
	/// </summary>
	/// <param name="searcher"></param>
	/// <returns></returns>
	private List<UserPrincipal>? FindUsers(PrincipalSearcher searcher)
	{
		var foundUsers = new List<UserPrincipal>();
		foreach (var foundUser in searcher.FindAll().OfType<UserPrincipal>())
		{
			var currentOuValues = foundUser.DistinguishedName
				.Split(',')
				.Select(x => x.Trim())
				.Where(x => x.StartsWith("OU="))
				.Select(x => x[3..])
				.ToHashSet();

			if (RequiredOuValues.IsSubsetOf(currentOuValues))
			{
				foundUsers.Add(foundUser);
			}
		}

		return foundUsers;
	}
}
