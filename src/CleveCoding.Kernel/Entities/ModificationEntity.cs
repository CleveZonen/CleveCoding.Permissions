#region

using System.ComponentModel.DataAnnotations.Schema;

#endregion

namespace CleveCoding.Kernel.Entities;

public abstract record ModificationEntity<TPrimaryKey> : CreationEntity<TPrimaryKey>, IHasModificationEntity
{
	/// <inheritdoc />
	public virtual DateTime? LastModifiedAt { get; set; }

	/// <inheritdoc />
	[Column(TypeName = "nvarchar(100)")]
	public virtual string? LastModifiedBy { get; set; }
}