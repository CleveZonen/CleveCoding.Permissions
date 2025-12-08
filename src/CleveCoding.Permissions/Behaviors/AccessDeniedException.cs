namespace CleveCoding.Permissions.Behaviors;

[Serializable]
public class AccessDeniedException : Exception
{
    public AccessDeniedException(string message) : base(message) { }
    public AccessDeniedException(string message, Exception ex) : base(message, ex) { }
}
