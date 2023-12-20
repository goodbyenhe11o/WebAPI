using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace igs_backend.Migrations
{
    public partial class addLogo : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Logo",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DesktopImgPath = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    MobileImgPath = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    PWAImgPath = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Logo", x => x.ID);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Logo");
        }
    }
}
