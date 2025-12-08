using CleveCoding.Permissions.Extensions;
using CleveCoding.PermissionsApp.Components;
using CleveCoding.PermissionsApp.Features.UserReviews;
using Microsoft.EntityFrameworkCore;
using System.Runtime.Versioning;

[assembly: SupportedOSPlatform("windows")]

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Add the Permissions.
builder.Services.AddPermissions<GetUserReviewsForIndexRequest>(config =>
{
    config.ConnectionString = builder.Configuration.GetConnectionString("DefaultConnection")!;
    config.AdminRoles = ["HRM-Administrators"];
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

// Use the Permissions.
app.UseAuthentication();
await app.UsePermissions();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
