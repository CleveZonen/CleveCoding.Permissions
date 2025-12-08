using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace CleveCoding.Permissions.Persistance.Internal;

/// <summary>
/// Mostly nessary to create migrations.
/// </summary>
internal class DesignTimeContextFactory : IDesignTimeDbContextFactory<PermissionDbContext>
{
	public PermissionDbContext CreateDbContext(string[] args)
	{
		var optionsBuilder = new DbContextOptionsBuilder<PermissionDbContext>();
		optionsBuilder.UseSqlServer("Server=RTDSQL4\\DEV;Database=CleveHRM;Trusted_Connection=false;persist security info=true;TrustServerCertificate=True;MultipleActiveResultSets=true;User ID=CLVCIS;Password=clvcis@d-");
		return new(optionsBuilder.Options);
	}
}
