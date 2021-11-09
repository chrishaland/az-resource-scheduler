using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Repository.Migrations
{
    public partial class Add_resource_stop_job : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ResourceStopJobs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    JobId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    StopAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ResourceId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ResourceStopJobs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ResourceStopJobs_Resources_ResourceId",
                        column: x => x.ResourceId,
                        principalTable: "Resources",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ResourceStopJobs_ResourceId",
                table: "ResourceStopJobs",
                column: "ResourceId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ResourceStopJobs");
        }
    }
}
