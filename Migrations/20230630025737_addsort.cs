using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace igs_backend.Migrations
{
    public partial class addsort : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Sort",
                table: "Group",
                type: "int",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Sort",
                table: "Group");
        }
    }
}
