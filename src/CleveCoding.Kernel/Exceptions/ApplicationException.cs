#region

using Newtonsoft.Json;

#endregion

namespace CleveCoding.Kernel.Exceptions;

/// <summary>
///     Exception thrown on specific Application Errors, may contain _DebugData in Data-property.
/// </summary>
[Serializable]
public class ApplicationException : Exception
{
    public ApplicationException(string message) : base(message)
    {
    }

    public ApplicationException(string message, Exception ex) : base(message, ex)
    {
    }

    public ApplicationException(string message, Exception ex, object? debugData) : base(message, ex)
    {
        Data.Add("_DebugData", debugData is not null ? JsonConvert.SerializeObject(debugData) : "NULL");
    }
}