using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Polaris.WMS.Migrations
{
    /// <inheritdoc />
    public partial class editreel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AppInventoryTransactions_AppReels_ReelId",
                table: "AppInventoryTransactions");

            migrationBuilder.RenameColumn(
                name: "ReelId",
                table: "AppTransferOrderDetails",
                newName: "ContainerId");

            migrationBuilder.RenameIndex(
                name: "IX_AppTransferOrderDetails_ReelId",
                table: "AppTransferOrderDetails",
                newName: "IX_AppTransferOrderDetails_ContainerId");

            migrationBuilder.RenameColumn(
                name: "ReelType",
                table: "AppReels",
                newName: "ContainerType");

            migrationBuilder.RenameColumn(
                name: "ReelNo",
                table: "AppReels",
                newName: "ContainerCode");

            migrationBuilder.RenameIndex(
                name: "IX_AppReels_ReelNo",
                table: "AppReels",
                newName: "IX_AppReels_ContainerCode");

            migrationBuilder.RenameColumn(
                name: "ReelId",
                table: "AppInventoryTransactions",
                newName: "ContainerId");

            migrationBuilder.RenameIndex(
                name: "IX_AppInventoryTransactions_ReelId",
                table: "AppInventoryTransactions",
                newName: "IX_AppInventoryTransactions_ContainerId");

            migrationBuilder.RenameColumn(
                name: "ReelId",
                table: "AppInventorys",
                newName: "ContainerId");

            migrationBuilder.RenameIndex(
                name: "IX_AppInventorys_ReelId",
                table: "AppInventorys",
                newName: "IX_AppInventorys_ContainerId");

            migrationBuilder.AddForeignKey(
                name: "FK_AppInventoryTransactions_AppReels_ContainerId",
                table: "AppInventoryTransactions",
                column: "ContainerId",
                principalTable: "AppReels",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AppInventoryTransactions_AppReels_ContainerId",
                table: "AppInventoryTransactions");

            migrationBuilder.RenameColumn(
                name: "ContainerId",
                table: "AppTransferOrderDetails",
                newName: "ReelId");

            migrationBuilder.RenameIndex(
                name: "IX_AppTransferOrderDetails_ContainerId",
                table: "AppTransferOrderDetails",
                newName: "IX_AppTransferOrderDetails_ReelId");

            migrationBuilder.RenameColumn(
                name: "ContainerType",
                table: "AppReels",
                newName: "ReelType");

            migrationBuilder.RenameColumn(
                name: "ContainerCode",
                table: "AppReels",
                newName: "ReelNo");

            migrationBuilder.RenameIndex(
                name: "IX_AppReels_ContainerCode",
                table: "AppReels",
                newName: "IX_AppReels_ReelNo");

            migrationBuilder.RenameColumn(
                name: "ContainerId",
                table: "AppInventoryTransactions",
                newName: "ReelId");

            migrationBuilder.RenameIndex(
                name: "IX_AppInventoryTransactions_ContainerId",
                table: "AppInventoryTransactions",
                newName: "IX_AppInventoryTransactions_ReelId");

            migrationBuilder.RenameColumn(
                name: "ContainerId",
                table: "AppInventorys",
                newName: "ReelId");

            migrationBuilder.RenameIndex(
                name: "IX_AppInventorys_ContainerId",
                table: "AppInventorys",
                newName: "IX_AppInventorys_ReelId");

            migrationBuilder.AddForeignKey(
                name: "FK_AppInventoryTransactions_AppReels_ReelId",
                table: "AppInventoryTransactions",
                column: "ReelId",
                principalTable: "AppReels",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
