using CleveCoding.Permissions.Extensions;
using CleveCoding.PermissionsApp.Components;
using CleveCoding.PermissionsApp.Features.UserReviews;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Add the Permissions.
builder.Services.AddPermissions<GetUserReviewsForIndexRequest>(builder.Configuration.GetConnectionString("DefaultConnection")!);

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
