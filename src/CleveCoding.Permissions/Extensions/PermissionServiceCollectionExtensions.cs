using System.Reflection;
using CleveCoding.Permissions.Behaviors;
using CleveCoding.Permissions.Circuits;
using CleveCoding.Permissions.Configurations;
using CleveCoding.Permissions.Middleware;
using CleveCoding.Permissions.Persistance;
using CleveCoding.Permissions.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Components.Server.Circuits;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace CleveCoding.Permissions.Extensions;

public static class PermissionServiceCollectionExtensions
{
	/// <summary>
	/// Wires up all the nessary services for the Permissions Package.
	/// </summary>
	/// <typeparam name="T">Used for MediatR to Register Services from Assembly and scan for PermissionDescription's.</typeparam>
	/// <param name="services"></param>
	/// <param name="configuration">Configurations for the Permissions.</param>
	/// <param name="mediatRConfiguration">When provided, registers into MediatR with AddMediatR()-call.</param>
	public static void AddPermissions<T>(this IServiceCollection services, Action<PermissionConfigurations> configuration, MediatRServiceConfiguration? mediatRConfiguration = null)
		where T : IRequirePermission
	{
		var permissionConfigurations = new PermissionConfigurations();
		configuration.Invoke(permissionConfigurations);

		// register the configuration
		services.AddSingleton(permissionConfigurations);

		// register the DbContext for the permissions,
		// EF Core is used for storage management.
		services.AddDbContextFactory<PermissionDbContext>(options =>
			options.UseSqlServer(permissionConfigurations.ConnectionString, sqlOptions =>
				sqlOptions.MigrationsAssembly(typeof(PermissionDbContext).Assembly.FullName)));

		// register the UserAccessor and PermissionEvaluator.
		services.AddHttpContextAccessor();
		services.AddScoped<IUserAccessor, UserAccessor>();
		services.AddScoped<IPermissionEvaluator, PermissionEvaluator>();

		// register the Services.
		services.AddMemoryCache();
		services.AddScoped<PermissionCache>();
		services.AddScoped<IUserLookupService, UserLookupService>();
		services.AddScoped<IPermissionService, PermissionService>();
		services.AddScoped<IUserDataAccessService, UserDataAccessService>();

		// register the UserContextInitializer
		// to populate the UserAccount with its permissions.
		services.AddTransient<UserContextInitializer>();
		services.AddTransient<ForbiddenExceptionHandler>();

		// register circuit handlers that handle the transition between prerender and interactive mode.
		services.AddScoped<CircuitHandler, UserCircuitHandler>();

		// register the MediatR permissions checks.
		if (mediatRConfiguration is not null)
		{
			/// register all MediatR handlers from the assembly of T
			/// into an existing MediatR configuration and add it to MediatR.

			mediatRConfiguration.RegisterServicesFromAssembly(typeof(T).Assembly);

			// register the behavior to verify permissions on each request.
			mediatRConfiguration.AddOpenBehavior(typeof(VerifyPermissionBehavior<,>));

			// register the behavior to log user data access on each request.
			mediatRConfiguration.AddOpenBehavior(typeof(UserDataAccessLogBehaviour<,>));

			services.AddMediatR(mediatRConfiguration);
		}
		else
		{
			/// register all MediatR handlers from the assembly of T
			services.AddMediatR(cfg =>
			{
				cfg.RegisterServicesFromAssembly(typeof(T).Assembly);

				// register the behavior to verify permissions on each request.
				cfg.AddOpenBehavior(typeof(VerifyPermissionBehavior<,>));

				// register the behavior to log user data access on each request.
				cfg.AddOpenBehavior(typeof(UserDataAccessLogBehaviour<,>));
			});
		}

		// scan for IRequirePermission to collect PermissionDescription
		// and register it in services as IEnumerable
		var assembly = typeof(T).Assembly;
		var interfaceType = typeof(IRequirePermission);
		var results = new List<PermissionDescription>();
		foreach (var type in assembly.DefinedTypes.Where(t => interfaceType.IsAssignableFrom(t) && !t.IsAbstract))
		{
			var prop = type.GetProperty(nameof(IRequirePermission.RequiredPermission), BindingFlags.Public | BindingFlags.Static);
			if (prop?.GetValue(null) is PermissionDescription permission)
			{
				results.Add(permission);
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
