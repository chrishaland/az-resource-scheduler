using Microsoft.EntityFrameworkCore.Migrations;

namespace Repository.Migrations
{
    public partial class Resource_table_splitting : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Kind",
                table: "Resources");

            migrationBuilder.RenameColumn(
                name: "NodePoolCount",
                table: "Resources",
                newName: "Capacity");

            migrationBuilder.AddColumn<string>(
                name: "Discriminator",
                table: "Resources",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Discriminator",
                table: "Resources");

            migrationBuilder.RenameColumn(
                name: "Capacity",
                table: "Resources",
                newName: "NodePoolCount");

            migrationBuilder.AddColumn<int>(
                name: "Kind",
                table: "Resources",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
