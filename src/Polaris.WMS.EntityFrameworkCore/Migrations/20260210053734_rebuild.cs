using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Polaris.WMS.Migrations
{
    /// <inheritdoc />
    public partial class rebuild : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AppInventoryTransactions_AppLocations_LocationId",
                table: "AppInventoryTransactions");

            migrationBuilder.DropIndex(
                name: "IX_AppInventoryTransactions_LocationId",
                table: "AppInventoryTransactions");

            migrationBuilder.DropColumn(
                name: "LocationId",
                table: "AppInventoryTransactions");

            migrationBuilder.RenameColumn(
                name: "QuantityChange",
                table: "AppInventoryTransactions",
                newName: "Quantity");

            migrationBuilder.AddColumn<Guid>(
                name: "WarehouseId",
                table: "AppLocations",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<string>(
                name: "CraftVersion",
                table: "AppInventoryTransactions",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<Guid>(
                name: "FromLocationId",
                table: "AppInventoryTransactions",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "FromWarehouseId",
                table: "AppInventoryTransactions",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Remark",
                table: "AppInventoryTransactions",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "SN",
                table: "AppInventoryTransactions",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "AppInventoryTransactions",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<Guid>(
                name: "ToLocationId",
                table: "AppInventoryTransactions",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ToWarehouseId",
                table: "AppInventoryTransactions",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_AppInventoryTransactions_FromLocationId",
                table: "AppInventoryTransactions",
                column: "FromLocationId");

            migrationBuilder.CreateIndex(
                name: "IX_AppInventoryTransactions_ToLocationId",
                table: "AppInventoryTransactions",
                column: "ToLocationId");

            migrationBuilder.AddForeignKey(
                name: "FK_AppInventoryTransactions_AppLocations_FromLocationId",
                table: "AppInventoryTransactions",
                column: "FromLocationId",
                principalTable: "AppLocations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AppInventoryTransactions_AppLocations_ToLocationId",
                table: "AppInventoryTransactions",
                column: "ToLocationId",
                principalTable: "AppLocations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AppInventoryTransactions_AppLocations_FromLocationId",
                table: "AppInventoryTransactions");

            migrationBuilder.DropForeignKey(
                name: "FK_AppInventoryTransactions_AppLocations_ToLocationId",
                table: "AppInventoryTransactions");

            migrationBuilder.DropIndex(
                name: "IX_AppInventoryTransactions_FromLocationId",
                table: "AppInventoryTransactions");

            migrationBuilder.DropIndex(
                name: "IX_AppInventoryTransactions_ToLocationId",
                table: "AppInventoryTransactions");

            migrationBuilder.DropColumn(
                name: "WarehouseId",
                table: "AppLocations");

            migrationBuilder.DropColumn(
                name: "CraftVersion",
                table: "AppInventoryTransactions");

            migrationBuilder.DropColumn(
                name: "FromLocationId",
                table: "AppInventoryTransactions");

            migrationBuilder.DropColumn(
                name: "FromWarehouseId",
                table: "AppInventoryTransactions");

            migrationBuilder.DropColumn(
                name: "Remark",
                table: "AppInventoryTransactions");

            migrationBuilder.DropColumn(
                name: "SN",
                table: "AppInventoryTransactions");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "AppInventoryTransactions");

            migrationBuilder.DropColumn(
                name: "ToLocationId",
                table: "AppInventoryTransactions");

            migrationBuilder.DropColumn(
                name: "ToWarehouseId",
                table: "AppInventoryTransactions");

            migrationBuilder.RenameColumn(
                name: "Quantity",
                table: "AppInventoryTransactions",
                newName: "QuantityChange");

            migrationBuilder.AddColumn<Guid>(
                name: "LocationId",
                table: "AppInventoryTransactions",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_AppInventoryTransactions_LocationId",
                table: "AppInventoryTransactions",
                column: "LocationId");

            migrationBuilder.AddForeignKey(
                name: "FK_AppInventoryTransactions_AppLocations_LocationId",
                table: "AppInventoryTransactions",
                column: "LocationId",
                principalTable: "AppLocations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}

