using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Polaris.WMS.Migrations
{
    /// <inheritdoc />
    public partial class _2602130001 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_AppInventoryTransactions_FromWarehouseId",
                table: "AppInventoryTransactions",
                column: "FromWarehouseId");

            migrationBuilder.CreateIndex(
                name: "IX_AppInventoryTransactions_ToWarehouseId",
                table: "AppInventoryTransactions",
                column: "ToWarehouseId");

            migrationBuilder.AddForeignKey(
                name: "FK_AppInventoryTransactions_AppWarehouses_FromWarehouseId",
                table: "AppInventoryTransactions",
                column: "FromWarehouseId",
                principalTable: "AppWarehouses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AppInventoryTransactions_AppWarehouses_ToWarehouseId",
                table: "AppInventoryTransactions",
                column: "ToWarehouseId",
                principalTable: "AppWarehouses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AppInventoryTransactions_AppWarehouses_FromWarehouseId",
                table: "AppInventoryTransactions");

            migrationBuilder.DropForeignKey(
                name: "FK_AppInventoryTransactions_AppWarehouses_ToWarehouseId",
                table: "AppInventoryTransactions");

            migrationBuilder.DropIndex(
                name: "IX_AppInventoryTransactions_FromWarehouseId",
                table: "AppInventoryTransactions");

            migrationBuilder.DropIndex(
                name: "IX_AppInventoryTransactions_ToWarehouseId",
                table: "AppInventoryTransactions");
        }
    }
}

