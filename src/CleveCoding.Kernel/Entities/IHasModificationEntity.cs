namespace CleveCoding.Kernel.Entities;

public interface IHasModificationEntity
{
    /// <summary>
    ///     Last time the object was modified.
    /// </summary>
    DateTime? LastModifiedAt { get; set; }

    /// <summary>
    ///     Last user to modify the object.
    /// </summary>
    string? LastModifiedBy { get; set; }
}