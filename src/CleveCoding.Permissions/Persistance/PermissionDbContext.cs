using CleveCoding.Permissions.Entities;
using Microsoft.EntityFrameworkCore;

namespace CleveCoding.Permissions.Persistance;

public class PermissionDbContext(DbContextOptions<PermissionDbContext> options) : DbContext(options)
{
	public DbSet<UserPermissionEntity> UserPermissions { get; set; } = null!;
	public DbSet<UserPermissionAuditEntity> UserPermissionAudits { get; set; } = null!;

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		modelBuilder.Entity<UserPermissionEntity>().ToTable(nameof(UserPermissions));
		modelBuilder.Entity<UserPermissionAuditEntity>().ToTable(nameof(UserPermissionAudits));

		base.OnModelCreating(modelBuilder);
	}

	public async Task MigrateAsync(CancellationToken cancellationToken = default)
	{
		await Database.MigrateAsync(cancellationToken);
	}
}
