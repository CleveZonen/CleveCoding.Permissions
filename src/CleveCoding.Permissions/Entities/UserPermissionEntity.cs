using System.ComponentModel.DataAnnotations.Schema;
using CleveCoding.Kernel.Attributes;
using CleveCoding.Kernel.Entities;

namespace CleveCoding.Permissions.Entities;

public record UserPermissionEntity : CreationEntity<int>
{
	/// <summary>
	/// Gets the unique identifier for the user associated with this entity.
	/// </summary>
	[Encrypted, Column(TypeName = "varchar(max)")]
	public string? UserId { get; init; }

	/// <summary>
	/// Gets the unique identifier of the associated role.
	/// </summary>
	[Encrypted, Column(TypeName = "varchar(max)")]
	public string? RoleId { get; init; }

	/// <summary>
	/// Should be exact match with IRequirePermission.PermissionDescription.Resource
	/// </summary>
	[Encrypted, Column(TypeName = "varchar(max)")]
	public string Resource { get; init; } = null!;

	/// <summary>
	/// Gets the action that gets performed.
	/// </summary>
	public UserActionType Action { get; init; }

	/// <summary>
	/// Indicates whether the user or role has access to the specified resource and action.
	/// </summary>
	public bool HasAccess { get; init; }
}
