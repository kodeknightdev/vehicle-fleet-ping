using Microsoft.EntityFrameworkCore.Migrations;

namespace FleetPing.Api.Migrations
{
    public partial class AddPlateIndex : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Vehicles_Plate",
                table: "Vehicles",
                column: "Plate",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Vehicles_Plate",
                table: "Vehicles");
        }
    }
}
