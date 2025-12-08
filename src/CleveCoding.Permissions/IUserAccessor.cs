namespace CleveCoding.Permissions;

public interface IUserAccessor
{
    UserAccount? CurrentUser { get; }
}
