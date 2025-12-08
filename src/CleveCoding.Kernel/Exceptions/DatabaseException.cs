#region

using Newtonsoft.Json;

#endregion

namespace CleveCoding.Kernel.Exceptions;

/// <summary>
///     Exception thrown when an database exception occurs in the UnitOfWork.
/// </summary>
[Serializable]
public class DatabaseException : ApplicationException
{
    public DatabaseException(string message) : base(message)
    {
    }

    public DatabaseException(string message, Exception inner) : base(message, inner)
    {
    }

    public DatabaseException(string message, Exception ex, object? debugData) : base(message, ex)
    {
        Data.Add("_DebugData", debugData is not null ? JsonConvert.SerializeObject(debugData) : "NULL");
    }
}