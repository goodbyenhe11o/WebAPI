using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace igs_backend.Migrations
{
    public partial class removeName : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Name",
                table: "Maintenance");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "Macquee");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "Maintenance",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "Macquee",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);
        }
    }
}
