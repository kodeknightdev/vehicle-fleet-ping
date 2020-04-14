using Microsoft.EntityFrameworkCore.Migrations;

namespace FleetPing.Api.Migrations
{
    public partial class AddServiceInterval : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ServiceIntervalKm",
                table: "Vehicles",
                nullable: false,
                defaultValue: 15000);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ServiceIntervalKm",
                table: "Vehicles");
        }
    }
}
