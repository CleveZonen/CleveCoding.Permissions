using System.ComponentModel.DataAnnotations.Schema;
using CleveCoding.Kernel.Attributes;
using CleveCoding.Kernel.Entities;
using CleveCoding.Permissions.Models;

namespace CleveCoding.Permissions.Entities;

public record UserPermissionEntity : CreationEntity<int>
{
	[Encrypted, Column(TypeName = "varchar(max)")]
	public string? UserId { get; init; }

	[Encrypted, Column(TypeName = "varchar(max)")]
	public string? RoleId { get; init; }

	/// <summary>
	/// Should be exact match with IRequirePermission.PermissionDescription.Resource
	/// </summary>
	[Encrypted, Column(TypeName = "varchar(max)")]
	public string Resource { get; init; } = null!;

	public UserActionType Action { get; init; }

	public bool HasAccess { get; init; }
}
