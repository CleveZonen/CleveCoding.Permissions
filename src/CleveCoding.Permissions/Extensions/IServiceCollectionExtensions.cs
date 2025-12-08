using CleveCoding.Permissions.Behaviors;
using CleveCoding.Permissions.Middleware;
using CleveCoding.Permissions.Models;
using CleveCoding.Permissions.Persistance;
using CleveCoding.Permissions.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace CleveCoding.Permissions.Extensions;

public static class IServiceCollectionExtensions
{
    public static void AddPermissions<T>(this IServiceCollection services, string connectionString)
        where T : IRequirePermission
    {
        // register the DbContext for the permissions,
        // EF Core is used for storage management.
        services.AddDbContext<PermissionDbContext>(options => options.UseSqlServer(connectionString,
            sqlServerOptionsAction: sqlOptions => sqlOptions.MigrationsAssembly(typeof(PermissionDbContext).Assembly.FullName)), ServiceLifetime.Transient);

        // register the PermissionService and its required classes.
        services.AddMemoryCache();
        services.AddScoped<PermissionCache>();
        services.AddScoped<IPermissionService, PermissionService>();

        // register the UserAccessor and its required classes.
        services.AddHttpContextAccessor();
        services.AddScoped<IUserAccessor, UserAccessor>();

        // register the UserContextInitializer
        // to populate the UserAccount with its permissions.
        services.AddTransient<UserContextInitializer>();
        services.AddTransient<IMiddleware, UserContextInitializer>();

        // register the MediatR permissions checks.
        services.AddMediatR(cfg =>
        {
            cfg.AddOpenBehavior(typeof(VerifyPermissionBehavior<,>));
        });

        // scan for IRequirePermission to collect PermissionDescription
        // and register it in services as IEnumerable
        var typeToAdd = typeof(T);
        var assembly = Assembly.GetAssembly(typeToAdd);
        var foundPermissions = new List<PermissionDescription>();
        foreach (var definedType in assembly!.DefinedTypes.Where(x => x.GetInterfaces().Contains(typeToAdd)))
        {
            if (definedType is IRequirePermission requirePermission)
            {
                foundPermissions.Add(requirePermission.RequiredPermission);
            }
        }

        services.AddSingleton(typeof(IEnumerable<IPermissionService>), foundPermissions);
    }

    public static async Task UsePermissions(this IApplicationBuilder app)
    {
        // register the initializer into the pipeline.
        app.UseMiddleware<UserContextInitializer>();

        // run the permissions migrations on startup.
        using var scope = app.ApplicationServices.CreateScope();
        await (await scope.ServiceProvider.GetRequiredService<IDbContextFactory<PermissionDbContext>>()
            .CreateDbContextAsync()).MigrateAsync();
    }
}
