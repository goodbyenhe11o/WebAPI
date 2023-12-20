using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace igs_backend.Migrations
{
    public partial class change : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserPermissions_Permissions_PermissionsID",
                table: "UserPermissions");

            migrationBuilder.AlterColumn<int>(
                name: "PermissionsID",
                table: "UserPermissions",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_UserPermissions_Permissions_PermissionsID",
                table: "UserPermissions",
                column: "PermissionsID",
                principalTable: "Permissions",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserPermissions_Permissions_PermissionsID",
                table: "UserPermissions");

            migrationBuilder.AlterColumn<int>(
                name: "PermissionsID",
                table: "UserPermissions",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_UserPermissions_Permissions_PermissionsID",
                table: "UserPermissions",
                column: "PermissionsID",
                principalTable: "Permissions",
                principalColumn: "ID");
        }
    }
}
