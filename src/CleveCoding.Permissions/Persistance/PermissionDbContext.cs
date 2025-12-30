using CleveCoding.Kernel.Cryptography;
using CleveCoding.Kernel.Extensions.Data;
using CleveCoding.Permissions.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace CleveCoding.Permissions.Persistance;

public class PermissionDbContext(DbContextOptions<PermissionDbContext> options) : DbContext(options)
{
	/// <summary>
	///     Converter to encrypt and decrypt _string_ values.
	/// </summary>
	private static readonly EncryptionValueConverter StringConverter = new();

	public DbSet<UserPermissionEntity> UserPermissions { get; set; } = null!;
	public DbSet<UserPermissionAuditEntity> UserPermissionAudits { get; set; } = null!;
	public DbSet<UserDataAccessLogEntity> UserDataAccessLogs { get; set; } = null!;

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		base.OnModelCreating(modelBuilder);

		modelBuilder.Entity<UserPermissionEntity>().ToTable(nameof(UserPermissions));
		modelBuilder.Entity<UserPermissionAuditEntity>().ToTable(nameof(UserPermissionAudits));
		modelBuilder.Entity<UserDataAccessLogEntity>().ToTable(nameof(UserDataAccessLogs));

		// use encryption on properties marked with the Encrypted-Attribute
		modelBuilder.UseEncryption<string>(StringConverter);
	}

	public async Task MigrateAsync(CancellationToken cancellationToken = default)
	{
		await Database.MigrateAsync(cancellationToken);
	}
}

internal sealed class EncryptionValueConverter(ConverterMappingHints? mappingHints = null)
	: ValueConverter<string?, string?>(
		x => Encryption.Encrypt(x),
		x => Encryption.Decrypt(x),
		mappingHints);
