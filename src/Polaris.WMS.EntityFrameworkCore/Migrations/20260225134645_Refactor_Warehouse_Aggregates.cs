using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Polaris.WMS.Migrations
{
    /// <inheritdoc />
    public partial class Refactor_Warehouse_Aggregates : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AppLocations_AppZones_ZoneId",
                table: "AppLocations");

            migrationBuilder.DropForeignKey(
                name: "FK_AppZones_AppWarehouses_WarehouseId",
                table: "AppZones");

            migrationBuilder.AddColumn<string>(
                name: "ConcurrencyStamp",
                table: "AppZones",
                type: "character varying(40)",
                maxLength: 40,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ExtraProperties",
                table: "AppZones",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ConcurrencyStamp",
                table: "AppLocations",
                type: "character varying(40)",
                maxLength: 40,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ExtraProperties",
                table: "AppLocations",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_AppLocations_WarehouseId",
                table: "AppLocations",
                column: "WarehouseId");

            migrationBuilder.CreateIndex(
                name: "IX_AppLocations_ZoneId",
                table: "AppLocations",
                column: "ZoneId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_AppLocations_WarehouseId",
                table: "AppLocations");

            migrationBuilder.DropIndex(
                name: "IX_AppLocations_ZoneId",
                table: "AppLocations");

            migrationBuilder.DropColumn(
                name: "ConcurrencyStamp",
                table: "AppZones");

            migrationBuilder.DropColumn(
                name: "ExtraProperties",
                table: "AppZones");

            migrationBuilder.DropColumn(
                name: "ConcurrencyStamp",
                table: "AppLocations");

            migrationBuilder.DropColumn(
                name: "ExtraProperties",
                table: "AppLocations");

            migrationBuilder.AddForeignKey(
                name: "FK_AppLocations_AppZones_ZoneId",
                table: "AppLocations",
                column: "ZoneId",
                principalTable: "AppZones",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AppZones_AppWarehouses_WarehouseId",
                table: "AppZones",
                column: "WarehouseId",
                principalTable: "AppWarehouses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}

