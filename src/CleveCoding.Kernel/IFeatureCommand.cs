namespace CleveCoding.Kernel;

/// <summary>
///     Register the features that are managed for restrictions.
///     Features that always should be fully accessible, can be omitted from appsettings registery
///     and dont need to implement the IFeature interface.
///     - https://github.com/microsoft/FeatureManagement-Dotnet
///     - https://learn.microsoft.com/en-us/azure/azure-app-configuration/use-feature-flags-dotnet-core
/// </summary>
/// <typeparam name="TResponse"></typeparam>
public interface IFeatureCommand<out TResponse> : ICommand<TResponse>
{
    /// <summary>
    ///     The feature name should be registered in appsettings.FeatureManagement and Application.Resources.Features.resx,
    ///     with the full request name as feature name. E.g. BuildExcelFileByRelationIdRequest
    /// </summary>
    string FeatureName { get; }
}