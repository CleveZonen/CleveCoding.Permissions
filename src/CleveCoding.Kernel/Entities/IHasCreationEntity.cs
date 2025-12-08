namespace CleveCoding.Kernel.Entities;

public interface IHasCreationEntity
{
    /// <summary>
    ///     The time the object was created.
    /// </summary>
    DateTime CreatedAt { get; set; }

    /// <summary>
    ///     The user the object was created by.
    /// </summary>
    string CreatedBy { get; set; }
}