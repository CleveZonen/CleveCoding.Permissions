using System.Runtime.Versioning;
using CleveCoding.Permissions.Extensions;
using CleveCoding.PermissionsApp.Components;
using CleveCoding.PermissionsApp.Features.UserReviews;
using Microsoft.AspNetCore.Authentication.Negotiate;
using Microsoft.EntityFrameworkCore;

[assembly: SupportedOSPlatform("windows")]

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
builder.Services.AddPermissions<GetUserReviewsForIndexRequest>(config =>
{
	config.ConnectionString = builder.Configuration.GetConnectionString("DefaultConnection")!;
	config.AdminRoles = ["HRM-Role-Administrators", "HRM-Role-Development"];
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
	app.UseExceptionHandler("/Error", createScopeForErrors: true);
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
