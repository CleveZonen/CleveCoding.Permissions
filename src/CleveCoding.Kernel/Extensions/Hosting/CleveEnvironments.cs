using Microsoft.Extensions.Hosting;

namespace CleveCoding.Kernel.Extensions.Hosting;

public static class CleveEnvironments
{
	/// <summary>
	///     The name of the System Environment Variable for .NET CORE Apps.
	/// </summary>
	public const string ENVIRONMENT_VARIABLE_NAME = "ASPNETCORE_ENVIRONMENT";

	/// <summary>
	///     Retrieve the Current Environment.
	/// </summary>
	/// <remarks>
	///     Production is used as default when environment variable is not set.
	/// </remarks>
	public static string Current => Environment.GetEnvironmentVariable(ENVIRONMENT_VARIABLE_NAME) ?? Production;

	/// <summary>
	///     Specifies the Development environment.
	/// </summary>
	/// <remarks>
	///     The development environment can enable features that shouldn't be exposed in production. Because of the performance cost, scope validation and dependency validation only happens in development.
	/// </remarks>
	public static readonly string Development = Environments.Development;
	public static bool IsDevelopment => string.Equals(Current, Development, StringComparison.OrdinalIgnoreCase);

	/// <summary>
	///     Specifies the Test environment.
	/// </summary>
	/// <remarks>
	///     The test environment can enable features that shouldn't be exposed in production. Because of the performance cost, scope validation and dependency validation only happens in test.
	/// </remarks>
	public static readonly string Test = "Test";
	public static bool IsTest => string.Equals(Current, Test, StringComparison.OrdinalIgnoreCase);

	/// <summary>
	///     Specifies the Acceptance environment.
	/// </summary>
	/// <remarks>
	///     The acceptance environment can be used to validate app changes before changing the environment to production.
	/// </remarks>
	public static readonly string Acceptance = "Acceptance";
	public static bool IsAcceptance => string.Equals(Current, Acceptance, StringComparison.OrdinalIgnoreCase);

	/// <summary>
	///     Specifies the Production environment.
	/// </summary>
	/// <remarks>
	///     The production environment should be configured to maximize security, performance, and application robustness.
	/// </remarks>
	public static readonly string Production = Environments.Production;
	public static bool IsProduction => string.Equals(Current, Production, StringComparison.OrdinalIgnoreCase);
}
