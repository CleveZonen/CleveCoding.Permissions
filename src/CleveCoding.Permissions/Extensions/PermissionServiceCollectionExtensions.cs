using CleveCoding.Permissions.Behaviors;
using CleveCoding.Permissions.Configurations;
using CleveCoding.Permissions.Middleware;
using CleveCoding.Permissions.Models;
using CleveCoding.Permissions.Persistance;
using CleveCoding.Permissions.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace CleveCoding.Permissions.Extensions;

public static class PermissionServiceCollectionExtensions
{
	public static void AddPermissions<T>(this IServiceCollection services, Action<PermissionConfigurations> configuration)
		where T : IRequirePermission
	{
		var permissionConfigurations = new PermissionConfigurations();
		configuration.Invoke(permissionConfigurations);

		// register the configuration
		services.AddSingleton(permissionConfigurations);

		// register the DbContext for the permissions,
		// EF Core is used for storage management.
		services.AddDbContextFactory<PermissionDbContext>(options => options.UseSqlServer(permissionConfigurations.ConnectionString), ServiceLifetime.Transient);
		services.AddDbContext<PermissionDbContext>(options => options.UseSqlServer(permissionConfigurations.ConnectionString,
			sqlServerOptionsAction: sqlOptions => sqlOptions.MigrationsAssembly(typeof(PermissionDbContext).Assembly.FullName)), ServiceLifetime.Transient);

		// register the Services.
		services.AddMemoryCache();
		services.AddScoped<PermissionCache>();
		services.AddScoped<IUserLookupService, UserLookupService>();
		services.AddScoped<IPermissionService, PermissionService>();

		// register the UserAccessor and its required classes.
		services.AddHttpContextAccessor();
		services.AddScoped<IUserAccessor, UserAccessor>();

		// register the UserContextInitializer
		// to populate the UserAccount with its permissions.
		services.AddTransient<UserContextInitializer>();
		services.AddTransient<ForbiddenExceptionHandler>();

		// register the MediatR permissions checks.
		services.AddMediatR(cfg =>
		{
			cfg.RegisterServicesFromAssembly(typeof(T).Assembly);
			cfg.AddOpenBehavior(typeof(VerifyPermissionBehavior<,>));
		});

		// scan for IRequirePermission to collect PermissionDescription
		// and register it in services as IEnumerable
		var assembly = typeof(T).Assembly;
		var interfaceType = typeof(IRequirePermission);
		var results = new List<PermissionDescription>();
		foreach (var type in assembly.DefinedTypes.Where(t => interfaceType.IsAssignableFrom(t) && !t.IsAbstract))
		{
			// create instance and read instance property
			if (Activator.CreateInstance(type) is IRequirePermission instance && instance.RequiredPermission != null)
			{
				results.Add(instance.RequiredPermission);
			}
		}

		// register as IEnumerable<PermissionDescription>
		services.AddSingleton<IEnumerable<PermissionDescription>>(results);
	}

	public static async Task UsePermissions(this IApplicationBuilder app)
	{
		// register the middlewares into the pipeline.
		app.UseMiddleware<UserContextInitializer>();
		app.UseMiddleware<ForbiddenExceptionHandler>();

		// run the permissions migrations on startup.
		using var scope = app.ApplicationServices.CreateScope();
		await (await scope.ServiceProvider.GetRequiredService<IDbContextFactory<PermissionDbContext>>()
			.CreateDbContextAsync()).MigrateAsync();
	}
}
