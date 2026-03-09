using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CleveCoding.Permissions.Migrations
{
	/// <inheritdoc />
	public partial class UpdateUserDataAccessLog : Migration
	{
		/// <inheritdoc />
		protected override void Up(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.AddColumn<DateTime>(
				name: "AnonymizedAt",
				table: "UserDataAccessLogs",
				type: "datetime2(2)",
				precision: 2,
				nullable: true);

			migrationBuilder.CreateIndex(
				name: "IX_UserDataAccessLogs_Anonymization",
				table: "UserDataAccessLogs",
				columns: ["DataCategory", "CreatedAt", "AnonymizedAt"]);
		}

		/// <inheritdoc />
		protected override void Down(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.DropIndex("IX_UserDataAccessLogs_Anonymization");

			migrationBuilder.DropColumn(
				name: "AnonymizedAt",
				table: "UserDataAccessLogs");
		}
	}
}
