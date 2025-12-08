namespace CleveCoding.Kernel.Entities;

/// <summary>
///     For entities that needs to implement Auditing.
///     The Factory pattern is used to create appropiate AuditEntities
///     for creation, modification and deletion.
/// </summary>
public interface IAuditedEntity
{
    /// <summary>
    ///     Create an Audit instance for newly created entities.
    /// </summary>
    /// <param name="user"></param>
    /// <returns></returns>
    IAuditEntity NewCreatedAudit(IUserAccount user);

    /// <summary>
    ///     Create an Audit instance for modified values in this Entity.
    /// </summary>
    /// <returns></returns>
    IAuditEntity NewChangedAudit(string propertyName, string propertyDisplayName, string? oldValue, string? newValue, IUserAccount user);

    /// <summary>
    ///     Create an Audit instance for deleted entities.
    /// </summary>
    /// <param name="user"></param>
    /// <returns></returns>
    IAuditEntity NewDeletedAudit(IUserAccount user);
}

/// <summary>
///     Implemented by Auditing Entities that record the changes of IAuditedEntity
/// </summary>
/// <typeparam name="TEntityPrimaryKey"></typeparam>
public interface IAuditEntity<TEntityPrimaryKey> : IAuditEntity
{
    /// <summary>
    ///     Id of the refered Entity.
    /// </summary>
    TEntityPrimaryKey SubjectId { get; set; }
}

/// <summary>
///     Entity Changes are audited and logged.
/// </summary>
public interface IAuditEntity : IEntity<long>, IHasCreationEntity
{
    string PropertyName { get; set; }
    string? OldValue { get; set; }
    string? NewValue { get; set; }
    string? AuditText { get; set; }
    string? Remarks { get; set; }
}