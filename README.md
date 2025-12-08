Here is the full HTML converted into clean, structured **Markdown**, preserving hierarchy, code blocks, formatting, and links.

---

# Welcome to the Permissions Package

The Permissions Package extends upon the IUserAccount in the
[CleveCoding.Kernel](https://github.com/CleveZonen/CleveCoding.Kernel) foundational package.
The **UserAccount** and **UserAccessor** are now found in the Permissions Package.

---

## Implementation

Implementation is relatively straightforward.

1. **Install** the **CleveCoding.Permissions** package with NuGet from the Cleve GitHub Packages.

2. **In `Program.cs`**, add the following in the ServiceCollection section, *after* the authentication calls:

   ```csharp
   // Add the Permissions. Ensure the call is after AddAuthentication!
   builder.Services.AddCascadingAuthenticationState();
   builder.Services.AddAuthentication(NegotiateDefaults.AuthenticationScheme)
       .AddNegotiate();
   builder.Services.AddAuthorization(options =>
   {
       // By default, all incoming requests will be authorized according to the default policy.
       options.FallbackPolicy = options.DefaultPolicy;
   });

   builder.Services.AddPermissions<GetItemsForIndexRequest>(config =>
   {
       config.ConnectionString = builder.Configuration.GetConnectionString("DefaultConnection")!;
       config.AdminRoles = RoleNames.Administrator;
   });
   ```

3. **In `Program.cs`**, add the following *after* `var app = builder.Build()`
   but *before* `app.MapRazorComponents<App>()`:

   ```csharp
   // Use the Permissions. Ensure the call is after UseAuthentication!
   app.UseAuthentication();
   app.UseAuthorization();
   await app.UsePermissions();
   ```

You can also inspect the full source code here:
[https://github.com/CleveZonen/CleveCoding.Permissions](https://github.com/CleveZonen/CleveCoding.Permissions)

---

## Usage

Using the Permissions Package is even simpler than implementing it.
Make sure to implement the **IRequirePermission** interface on the MediatR Requests/Commands.

**Resource:** Logical name for resources/modules
**Action:** User action defined in `UserActionType` enum
**Description:** Display text for the permissions control panel UI

```csharp
public record GetItemsForIndexRequest : ICommand<Response>, IRequirePermission
{
    public PermissionDescription RequiredPermission =>
        new(nameof(Item), UserActionType.ViewIndex, "Access to items index.");
}
```

---

## Notes

* `UserAccount` and `UserAccessor` are managed in this package.
* Permissions are discovered dynamically by scanning for `IRequirePermission` in the assembly that contains the type you pass to `AddPermissions<T>()`.
* User and role permissions are stored and managed via Entity Framework Core.
* Database migrations are checked and applied automatically on startup.
* All permission changes are audited (available through `IPermissionService`).
* User and role permissions are cached using `IMemoryCache`.
* Permission verification is automatically performed in the MediatR pipeline.
* Users with *Administrator roles* are exempt from permission verification.
* `UserAccessor.CurrentUser` is automatically populated via `UserContextInitializer` Middleware.
* User and role permissions are merged when populating `CurrentUser`, with **user permissions taking priority**.

---

## Access via Dependency Injection

| Service                              | Description                                                            |
| ------------------------------------ | ---------------------------------------------------------------------- |
| `IEnumerable<PermissionDescription>` | All discovered permissions from `IRequirePermission` implementations.  |
| `IPermissionService`                 | Manage user/role permissions and read audits.                          |
| `IUserAccessor`                      | Get the current user and check admin status.                           |
| `PermissionConfigurations`           | Contains the permissions configuration (ConnectionString, AdminRoles). |
| `IUserContextInitializer`            | Middleware to populate UserAccessor, but can also be called manually.  |

---

If you want any refinements—styling, screenshots, badges, or turning this into a full README.md—just let me know!
