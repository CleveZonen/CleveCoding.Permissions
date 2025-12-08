namespace CleveCoding.Kernel.Entities;

public abstract record Entity<TPrimaryKey> : IEntity<TPrimaryKey>
{
	/// <inheritdoc />
	public TPrimaryKey Id { get; set; } = default!;

	/// <inheritdoc />
	public bool IsTransient()
	{
		if (EqualityComparer<TPrimaryKey>.Default.Equals(Id, default)) return true;
		if (typeof(TPrimaryKey) == typeof(int)) return Convert.ToInt32(Id) <= 0;
		if (typeof(TPrimaryKey) == typeof(long)) return Convert.ToInt64(Id) <= 0;

		return false;
	}

	/// <inheritdoc />
	public override int GetHashCode() => Id!.GetHashCode();
}