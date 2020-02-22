using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace FleetPing.Api.Migrations
{
    public partial class AddMaintenanceLogs : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MaintenanceLogs",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    VehicleId = table.Column<int>(nullable: false),
                    Date = table.Column<DateTime>(nullable: false),
                    Type = table.Column<string>(maxLength: 20, nullable: false),
                    OdometerKm = table.Column<int>(nullable: false),
                    Cost = table.Column<decimal>(type: "numeric(10,2)", nullable: false),
                    Mechanic = table.Column<string>(maxLength: 80, nullable: true),
                    Notes = table.Column<string>(maxLength: 1000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MaintenanceLogs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MaintenanceLogs_Vehicles_VehicleId",
                        column: x => x.VehicleId,
                        principalTable: "Vehicles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MaintenanceLogs_VehicleId_Date",
                table: "MaintenanceLogs",
                columns: new[] { "VehicleId", "Date" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MaintenanceLogs");
        }
    }
}
