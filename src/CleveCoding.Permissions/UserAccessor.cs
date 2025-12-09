using System.DirectoryServices.AccountManagement;
using System.Security.Principal;
using CleveCoding.Kernel;
using CleveCoding.Permissions.Configurations;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Http;

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
	Task TryLoadUserAsync(bool forceReload = false, bool callFromMiddleware = false);

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
		_subscription = applicationState.RegisterOnPersisting(() =>
		{
			_applicationState.PersistAsJson(nameof(CurrentUser), CurrentUser);
			return Task.CompletedTask;
		});
	}

	/// <inheritdoc/>
	public async Task TryLoadUserAsync(bool forceReload = false, bool callFromMiddleware = false)
	{
		if (CurrentUser is not null && !forceReload) return;

		if (_applicationState is not null)
		{
			// get user from application state, persisted between rendermodes (between prerender- and server runs; Interactive-mode)
			var currentStateFound = _applicationState.TryTakeFromJson(nameof(CurrentUser), out UserAccount? foundUser);
			if (currentStateFound && foundUser is not null)
			{
				CurrentUser = foundUser;
				return;
			}
		}

		if (_httpContextAccessor.HttpContext is not null)
		{
			// get user from the HttpContext directly; NOT AVAILABLE in Interactive-mode.
			var principal = _httpContextAccessor.HttpContext?.User;
			if (principal is not null && principal.Identity is not null && principal.Identity.IsAuthenticated)
			{
				CurrentUser = LoadUser(principal.Identity);
				return;
			}
		}

		if (!callFromMiddleware)
		{
			// get user from authentication state provider
			var authenticationState = await _authenticationStateProvider.GetAuthenticationStateAsync();
			if (authenticationState.User.Identity?.IsAuthenticated == true)
			{
				CurrentUser = LoadUser(authenticationState.User.Identity);
			}
		}
	}

	/// <inheritdoc/>
	public UserAccount LoadUser(IIdentity identity)
	{
		// reset current user.
		var currentUser = new UserAccount();

		if (identity.Name == null) return currentUser;

		// find the domain and username.
		var identityArr = identity.Name.Split('\\');
		string domain;
		if (identityArr.Length > 1)
		{
			domain = identityArr[0];
			currentUser.AccountName = identityArr[1].ToUpper();
			currentUser.UserName = identityArr[1];
		}
		else
		{
			domain = Environment.UserDomainName;
			currentUser.UserName = identityArr[0];
		}

		// get the user information.
		if (string.Equals(domain, Environment.UserDomainName, StringComparison.OrdinalIgnoreCase))
		{
			try
			{
				var userPrincipal = UserPrincipal.FindByIdentity(new(ContextType.Domain, domain), identity.Name!);
				if (userPrincipal is null) return currentUser;

				currentUser.FirstName = userPrincipal.GivenName;
				currentUser.MiddleName = userPrincipal.MiddleName;
				currentUser.LastName = userPrincipal.Surname;
				currentUser.UserName = userPrincipal.DisplayName;
				currentUser.Description = userPrincipal.Description;
				currentUser.TelephoneNumber = userPrincipal.VoiceTelephoneNumber;
				currentUser.EmailAddress = userPrincipal.EmailAddress;
			}
			catch
			{
				// ignored
			}
		}

		// find the user roles.
		if (identity is not WindowsIdentity windowsIdentity) return currentUser;
		if (windowsIdentity.Groups is null) return currentUser;

		currentUser.Roles = windowsIdentity.Groups
			.Select(x => x.Translate(typeof(NTAccount)).Value.Split('\\'))
			.Where(x => x.Length > 1)
			.Select(x => x[1]);

		return currentUser;
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

	public void Dispose()
	{
		_subscription.Dispose();
	}
}
