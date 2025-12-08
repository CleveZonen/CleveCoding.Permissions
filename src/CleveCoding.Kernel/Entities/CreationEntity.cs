#region

using System.ComponentModel.DataAnnotations.Schema;

#endregion

namespace CleveCoding.Kernel.Entities;

public abstract record CreationEntity<TPrimaryKey> : Entity<TPrimaryKey>, IHasCreationEntity
{
	/// <inheritdoc />
	public virtual DateTime CreatedAt { get; set; } = DateTime.UtcNow;

	/// <inheritdoc />
	[Column(TypeName = "nvarchar(100)")]
	public virtual string CreatedBy { get; set; } = null!;
}