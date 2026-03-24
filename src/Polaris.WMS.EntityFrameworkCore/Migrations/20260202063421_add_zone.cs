using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Polaris.WMS.Migrations
{
    /// <inheritdoc />
    public partial class add_zone : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AppLocations_AppWarehouses_WarehouseId",
                table: "AppLocations");

            migrationBuilder.AddColumn<Guid>(
                name: "ZoneId",
                table: "AppLocations",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateTable(
                name: "AppZones",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    WarehouseId = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false, comment: "库区名称"),
                    Code = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false, comment: "库区编码")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppZones", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AppZones_AppWarehouses_WarehouseId",
                        column: x => x.WarehouseId,
                        principalTable: "AppWarehouses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AppLocations_ZoneId",
                table: "AppLocations",
                column: "ZoneId");

            migrationBuilder.CreateIndex(
                name: "IX_AppZones_WarehouseId_Code",
                table: "AppZones",
                columns: new[] { "WarehouseId", "Code" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_AppLocations_AppZones_ZoneId",
                table: "AppLocations",
                column: "ZoneId",
                principalTable: "AppZones",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AppLocations_AppZones_ZoneId",
                table: "AppLocations");

            migrationBuilder.DropTable(
                name: "AppZones");

            migrationBuilder.DropIndex(
                name: "IX_AppLocations_ZoneId",
                table: "AppLocations");

            migrationBuilder.DropColumn(
                name: "ZoneId",
                table: "AppLocations");

            migrationBuilder.AddForeignKey(
                name: "FK_AppLocations_AppWarehouses_WarehouseId",
                table: "AppLocations",
                column: "WarehouseId",
                principalTable: "AppWarehouses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}

