using Microsoft.EntityFrameworkCore.Migrations;

namespace Quasar.Data.Migrations
{
    public partial class AddPublicIdProductEntity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CoverPublicId",
                table: "Products",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CoverPublicId",
                table: "Products");
        }
    }
}
