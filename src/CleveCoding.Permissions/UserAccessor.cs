using CleveCoding.Kernel;
using CleveCoding.Permissions.Configurations;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Http;
using System.DirectoryServices.AccountManagement;
using System.Security.Principal;

namespace CleveCoding.Permissions;

public interface IUserAccessor
{
	/// <summary>
	/// Current authorized user.
	/// </summary>
	UserAccount? CurrentUser { get; }

	/// <summary>
	///     Find and retrieve the user information into the CurrentUser.
	///     The data is searched in Persistent Component State, Authentication State and the HttpContext.
	/// </summary>
	/// <param name="forceReload"></param>
	/// <returns></returns>
	/// <exception cref="InvalidOperationException"></exception>
	Task TryLoadUserAsync(bool forceReload = false);

	/// <summary>
	/// Load UserAccount based on user identity (Claims Principle).
	/// </summary>
	/// <param name="identity"></param>
	/// <returns></returns>
	UserAccount LoadUser(IIdentity identity);

	/// <summary>
	/// Check if the given user is in the administrator role.
	/// </summary>
	/// <param name="user"></param>
	/// <returns></returns>
	bool IsAdmin(IUserAccount user);

	/// <summary>
	/// Check if the given user is in the administrator role.
	/// </summary>
	/// <param name="user"></param>
	/// <returns></returns>
	bool IsAdmin(UserPrincipal user);
}

public sealed class UserAccessor : IDisposable, IUserAccessor
{
	private readonly PersistentComponentState _applicationState;
	private readonly AuthenticationStateProvider _authenticationStateProvider;
	private readonly IHttpContextAccessor _httpContextAccessor;
	private readonly PersistingComponentStateSubscription _subscription;
	private readonly PermissionConfigurations _configurations;

	/// <summary>
	///     Get the current loaded user.
	/// </summary>
	public UserAccount? CurrentUser { get; private set; }

	/// <summary>
	///     Verification if the CurrentUser is not null.
	/// </summary>
	public bool HasUser => CurrentUser is not null;

	public UserAccessor(IHttpContextAccessor httpContextAccessor,
		AuthenticationStateProvider authenticationStateProvider,
		PersistentComponentState applicationState,
		PermissionConfigurations configurations)
	{
		_httpContextAccessor = httpContextAccessor;
		_authenticationStateProvider = authenticationStateProvider;
		_applicationState = applicationState;
		_configurations = configurations;
		_subscription = applicationState.RegisterOnPersisting(PersistAsync);
	}

	/// <inheritdoc/>
	public async Task TryLoadUserAsync(bool forceReload = false)
	{
		if (CurrentUser is not null && !forceReload) return;

		if (_applicationState is not null)
		{
			// try get user from application state, persisted between rendermodes (between prerender- and server runs; Interactive-mode)
			var currentStateFound = _applicationState.TryTakeFromJson(nameof(CurrentUser), out UserAccount? foundUser);
			if (currentStateFound && foundUser is not null)
			{
				CurrentUser = foundUser;
				return;
			}
		}

		if (_httpContextAccessor.HttpContext is not null)
		{
			// try get user from the HttpContext directly; NOT AVAILABLE in Interactive-mode.
			var principal = _httpContextAccessor.HttpContext?.User;
			if (principal is not null && principal.Identity is not null && principal.Identity.IsAuthenticated)
			{
				CurrentUser = LoadUser(principal.Identity);
				return;
			}
		}

		// try get user from authentication state provider.
		var authenticationState = await _authenticationStateProvider.GetAuthenticationStateAsync();
		if (authenticationState.User.Identity?.IsAuthenticated == true)
		{
			CurrentUser = LoadUser(authenticationState.User.Identity);
		}
	}

	/// <inheritdoc/>
	public UserAccount LoadUser(IIdentity identity)
	{
		// reset current user.
		var loadedAccount = new UserAccount();

		if (identity.Name == null) return loadedAccount;

		// find the domain and username.
		var identityArr = identity.Name.Split('\\');
		string domain;
		if (identityArr.Length > 1)
		{
			domain = identityArr[0];
			loadedAccount.AccountName = identityArr[1];
			loadedAccount.UserName = identityArr[1];
		}
		else
		{
			domain = Environment.UserDomainName;
			loadedAccount.UserName = identityArr[0];
		}

		// get the user information.
		if (string.Equals(domain, Environment.UserDomainName, StringComparison.OrdinalIgnoreCase))
		{
			try
			{
				var userPrincipal = UserPrincipal.FindByIdentity(new(ContextType.Domain, domain), identity.Name!);
				if (userPrincipal is null) return loadedAccount;

				loadedAccount.FirstName = userPrincipal.GivenName;
				loadedAccount.MiddleName = userPrincipal.MiddleName;
				loadedAccount.LastName = userPrincipal.Surname;
				loadedAccount.UserName = userPrincipal.DisplayName;
				loadedAccount.Description = userPrincipal.Description;
				loadedAccount.TelephoneNumber = userPrincipal.VoiceTelephoneNumber;
				loadedAccount.EmailAddress = userPrincipal.EmailAddress;
			}
			catch
			{
				// ignored
			}
		}

		// find the user roles.
		if (identity is not WindowsIdentity windowsIdentity) return loadedAccount;
		if (windowsIdentity.Groups is null) return loadedAccount;

		loadedAccount.Roles = windowsIdentity.Groups
			.Select(x => x.Translate(typeof(NTAccount)).Value.Split('\\'))
			.Where(x => x.Length > 1)
			.Select(x => x[1]);

		return loadedAccount;
	}

	/// <inheritdoc/>
	public bool IsAdmin(IUserAccount user)
	{
		return user.IsInRoles(_configurations.AdminRoles);
	}

	/// <inheritdoc/>
	public bool IsAdmin(UserPrincipal user)
	{
		return user.GetGroups()
			.OfType<GroupPrincipal>()
			.Any(g => _configurations.AdminRoles.Contains(g.Name, StringComparer.OrdinalIgnoreCase));
	}

	private Task PersistAsync()
	{
		if (_applicationState is null || CurrentUser is null)
		{
			return Task.CompletedTask;
		}

		_applicationState.PersistAsJson(nameof(CurrentUser), CurrentUser);

		return Task.CompletedTask;
	}

	public void Dispose()
	{
		_subscription.Dispose();
	}
}
