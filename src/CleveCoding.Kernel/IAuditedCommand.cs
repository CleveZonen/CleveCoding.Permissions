namespace CleveCoding.Kernel;

public interface IAuditedCommand<out TResponse> : ICommand<TResponse>
{
    IUserAccount User { get; set; }
}