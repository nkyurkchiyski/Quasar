using Microsoft.EntityFrameworkCore.Migrations;

namespace Quasar.Data.Migrations
{
    public partial class AddPublicIdImageEntity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PublicId",
                table: "Images",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PublicId",
                table: "Images");
        }
    }
}
