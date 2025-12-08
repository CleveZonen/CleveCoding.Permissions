using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Http;
using System.DirectoryServices.AccountManagement;
using System.Security.Principal;

namespace CleveCoding.Permissions;

public sealed class UserAccessor : IDisposable, IUserAccessor
{
    private readonly PersistentComponentState _applicationState;
    private readonly AuthenticationStateProvider _authenticationStateProvider;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly NavigationManager _navigation;
    private readonly PersistingComponentStateSubscription _subscription;

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
        NavigationManager navigation)
    {
        _navigation = navigation;
        _httpContextAccessor = httpContextAccessor;
        _authenticationStateProvider = authenticationStateProvider;
        _applicationState = applicationState;
        _subscription = applicationState.RegisterOnPersisting(() =>
        {
            _applicationState.PersistAsJson(nameof(CurrentUser), CurrentUser);
            return Task.CompletedTask;
        });
    }

    public void Dispose()
    {
        _subscription.Dispose();
    }

    /// <summary>
    ///     Find and retrieve the user information into the CurrentUser.
    ///     The data is searched in Persistent Component State, Authentication State and the HttpContext.
    /// </summary>
    /// <param name="forceReload"></param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    public async Task TryLoadUserAsync(bool forceReload = false)
    {
        if (CurrentUser is not null && !forceReload) return;

        // get user from application state, persisted between rendermodes (between prerender- and server runs; Interactive-mode)
        var currentStateFound = _applicationState.TryTakeFromJson(nameof(CurrentUser), out UserAccount? foundUser);
        if (currentStateFound && foundUser is not null)
        {
            CurrentUser = foundUser;
            return;
        }

        // get user from authentication state provider
        var authenticationState = await _authenticationStateProvider.GetAuthenticationStateAsync();
        if (authenticationState.User.Identity?.IsAuthenticated == true)
        {
            TryLoadUserFromIdentity(authenticationState.User.Identity);
            return;
        }

        // get user from the HttpContext directly; NOT AVAILABLE in Interactive-mode.
        var principal = _httpContextAccessor.HttpContext?.User ??
                        throw new InvalidOperationException($"{nameof(TryLoadUserAsync)} requires access to an {nameof(HttpContext)}.");

        if (principal.Identity is not null && principal.Identity.IsAuthenticated)
        {
            TryLoadUserFromIdentity(principal.Identity);
        }
    }

    /// <summary>
    /// Lookup in AD to find user information.
    /// </summary>
    /// <param name="identity"></param>
    private void TryLoadUserFromIdentity(IIdentity identity)
    {
        CurrentUser = new UserAccount();

        if (identity.Name == null)
        {
            return;
        }

        var identityArr = identity.Name.Split('\\');
        string domain;

        if (identityArr.Length > 1)
        {
            domain = identityArr[0];

            CurrentUser.AccountName = identityArr[1].ToUpper();
            CurrentUser.UserName = identityArr[1];
        }
        else
        {
            domain = Environment.UserDomainName;
            CurrentUser.UserName = identityArr[0];
        }

        if (string.Equals(domain, Environment.UserDomainName, StringComparison.OrdinalIgnoreCase))
            try
            {
                var userPrincipal = UserPrincipal.FindByIdentity(new(ContextType.Domain, domain), identity.Name!);
                if (userPrincipal is null) return;

                CurrentUser.FirstName = userPrincipal.GivenName;
                CurrentUser.MiddleName = userPrincipal.MiddleName;
                CurrentUser.LastName = userPrincipal.Surname;
                CurrentUser.UserName = userPrincipal.DisplayName;
                CurrentUser.Description = userPrincipal.Description;
                CurrentUser.TelephoneNumber = userPrincipal.VoiceTelephoneNumber;
                CurrentUser.EmailAddress = userPrincipal.EmailAddress;
            }
            catch
            {
                // ignored
            }

        if (identity is not WindowsIdentity windowsIdentity) return;

        if (windowsIdentity.Groups is null) return;

        CurrentUser.Roles = windowsIdentity.Groups
            .Select(x => x.Translate(typeof(NTAccount)).Value.Split('\\'))
            .Where(x => x.Length > 1)
            .Select(x => x[1]);
    }
}