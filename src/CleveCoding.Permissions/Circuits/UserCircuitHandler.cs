using CleveCoding.Permissions.Middleware;
using Microsoft.AspNetCore.Components.Server.Circuits;

namespace CleveCoding.Permissions.Circuits;

/// <summary>
/// The UserCircuit Handler reinitializes the UserAccessor,
/// this ensures the CurrentUser in UserAccessor is loaded
/// in Blazor Interactive mode.
/// </summary>
/// <param name="initializer"></param>
public class UserCircuitHandler(UserContextInitializer initializer) : CircuitHandler
{
	private readonly UserContextInitializer _initializer = initializer;

	public override async Task OnCircuitOpenedAsync(Circuit circuit, CancellationToken cancellationToken)
	{
		await _initializer.InitializeAsync();
	}
}

