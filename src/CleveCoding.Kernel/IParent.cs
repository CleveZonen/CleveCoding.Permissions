namespace CleveCoding.Kernel;

public interface IParent<TChild, TParentKey, TPrimaryKey> : IDataModel<TPrimaryKey>
	where TChild : IParent<TChild, TParentKey, TPrimaryKey>
{
	/// <summary>
	///     Primary key of the parent-object.
	/// </summary>
	TParentKey? ParentId { get; set; }

	/// <summary>
	///     All the children of this parent-object.
	/// </summary>
	IList<TChild>? Children { get; set; }
}