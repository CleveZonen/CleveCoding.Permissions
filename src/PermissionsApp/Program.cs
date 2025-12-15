using System.Runtime.Versioning;
using CleveCoding.Kernel.Cryptography;
using CleveCoding.Permissions.Extensions;
using CleveCoding.PermissionsApp.Components;
using CleveCoding.PermissionsApp.Features.Item;
using Microsoft.AspNetCore.Authentication.Negotiate;
using Microsoft.EntityFrameworkCore;

[assembly: SupportedOSPlatform("windows")]

#region [ ENCRYPTION ]

// <important>
// 	The encryption key is used to encrypted and decrypt data.
// 	Without the key, encrypted data can NOT be decrypted. 
//  DO NOT CHANGE EVER!!
// </important>
Encryption.SetKey("7Cxt^ekhL@#*(@!ew$3ew.sd;,asd234");

#endregion [ ENCRYPTION ]

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
	.AddInteractiveServerComponents();

// Add HttpContext Accessor service.
builder.Services.AddHttpContextAccessor();

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
	config.ErrorPageUrl = "/errors/forbidden";
	config.AdminRoles = ["HRM-Role-Administrators", "HRM-Role-Development"];
	config.Roles = ["HRM-Role-Administrators", "HRM-Role-Development", "HRM-Role-Secretary"];
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
	// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
	app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

// Use the Permissions. Ensure the call is after UseAuthentication!
app.UseAuthentication();
app.UseAuthorization();
await app.UsePermissions();

app.MapRazorComponents<App>()
	.AddInteractiveServerRenderMode();

app.Run();
