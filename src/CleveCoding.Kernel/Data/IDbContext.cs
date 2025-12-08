namespace CleveCoding.Kernel.Data;

public interface IDbContext
{
    int SaveChanges();
    int SaveChanges(IUserAccount user);
    Task<int> SaveChangesAsync();
    Task<int> SaveChangesAsync(IUserAccount user);
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    Task<int> SaveChangesAsync(IUserAccount user, CancellationToken cancellationToken);
}