namespace CleveCoding.Kernel;

public interface IDataModel<TPrimaryKey>
{
	/// <summary>
	///     Primary key of the object.
	/// </summary>
	TPrimaryKey Id { get; set; }
}

/// <summary>
/// Simple marker interface to ensure an Id field.
/// </summary>
public interface IDataModel : IDataModel<int> { }
