using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CleveCoding.Permissions.Migrations
{
    /// <inheritdoc />
    public partial class UpdateUserPermission : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ActionId",
                table: "UserPermissions",
                type: "varchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ActionId",
                table: "UserPermissionAudits",
                type: "varchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ActionId",
                table: "UserPermissions");

            migrationBuilder.DropColumn(
                name: "ActionId",
                table: "UserPermissionAudits");
        }
    }
}
