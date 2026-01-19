namespace CleveCoding.Permissions;

/// <summary>
/// Implemented by MediatR's IFeatureCommand<> to 
/// define which features/handlers require permission.
/// 
/// Will be used in MediatR's pipeline to 
/// verify the requester has the right permission.
/// </summary>
public interface IRequirePermission
{
	/// <summary>
	/// Defines which permission is nessary to
	/// access this resource.
	/// </summary>
	static abstract PermissionDescription RequiredPermission { get; }
}
