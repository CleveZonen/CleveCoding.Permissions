using System.DirectoryServices.AccountManagement;
using System.Runtime.Versioning;

[assembly: SupportedOSPlatform("windows")]

namespace CleveCoding.Permissions.Services;

public class UserLookupService
{
    /// <summary>
    /// Get list of all the current registered users in Active Directory.
    /// </summary>
    /// <returns></returns>
    public static IList<UserPrincipal>? GetUsers()
    {
        return FindUsers(new PrincipalSearcher(new UserPrincipal(new(ContextType.Domain))
        {
            UserPrincipalName = "*@*",
            Enabled = true
        }));
    }

    /// <summary>
    /// Get list of all the current registered users in Active Directory from an role.
    /// </summary>
    /// <param name="roleId"></param>
    /// <returns></returns>
    public static IList<UserPrincipal>? GetUsersInRole(string roleId)
    {
        return FindUsers(new PrincipalSearcher(new UserPrincipal(new(ContextType.Domain))
        {
            UserPrincipalName = "*@*",
            Enabled = true
        }))?.Where(x => x.GetGroups().OfType<GroupPrincipal>().Any(g => g.Name == roleId))
            .ToList();
    }

    /// <summary>
    /// Get user information from Active Directory based on Principal.SamAccountName.
    /// </summary>
    /// <param name="accountName"></param>
    /// <returns></returns>
    public static UserPrincipal? GetUser(string accountName)
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
    private static List<UserPrincipal>? FindUsers(PrincipalSearcher searcher)
    {
        var requiredOuValues = new HashSet<string>(["Users"]);
        var foundUsers = new List<UserPrincipal>();
        foreach (var foundUser in searcher.FindAll().OfType<UserPrincipal>())
        {
            var currentOuValues = foundUser.DistinguishedName
                .Split(',')
                .Select(x => x.Trim())
                .Where(x => x.StartsWith("OU="))
                .Select(x => x[3..])
                .ToHashSet();

            if (requiredOuValues.IsSubsetOf(currentOuValues))
            {
                foundUsers.Add(foundUser);
            }
        }

        return foundUsers;
    }
}
