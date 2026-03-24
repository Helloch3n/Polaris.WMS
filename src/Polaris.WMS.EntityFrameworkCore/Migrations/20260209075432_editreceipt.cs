using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Polaris.WMS.Migrations
{
    /// <inheritdoc />
    public partial class editreceipt : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ReceiptDetails_AppProducts_ProductId",
                table: "ReceiptDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_ReceiptDetails_AppProducts_ProductId1",
                table: "ReceiptDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_ReceiptDetails_AppReels_ReelId",
                table: "ReceiptDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_ReceiptDetails_AppReels_ReelId1",
                table: "ReceiptDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_ReceiptDetails_Receipts_ReceiptId",
                table: "ReceiptDetails");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Receipts",
                table: "Receipts");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ReceiptDetails",
                table: "ReceiptDetails");

            migrationBuilder.RenameTable(
                name: "Receipts",
                newName: "AppReceipts");

            migrationBuilder.RenameTable(
                name: "ReceiptDetails",
                newName: "AppReceiptDetails");

            migrationBuilder.RenameIndex(
                name: "IX_Receipts_BillNo",
                table: "AppReceipts",
                newName: "IX_AppReceipts_BillNo");

            migrationBuilder.RenameIndex(
                name: "IX_ReceiptDetails_ReelId1",
                table: "AppReceiptDetails",
                newName: "IX_AppReceiptDetails_ReelId1");

            migrationBuilder.RenameIndex(
                name: "IX_ReceiptDetails_ReelId",
                table: "AppReceiptDetails",
                newName: "IX_AppReceiptDetails_ReelId");

            migrationBuilder.RenameIndex(
                name: "IX_ReceiptDetails_ReceiptId",
                table: "AppReceiptDetails",
                newName: "IX_AppReceiptDetails_ReceiptId");

            migrationBuilder.RenameIndex(
                name: "IX_ReceiptDetails_ProductId1",
                table: "AppReceiptDetails",
                newName: "IX_AppReceiptDetails_ProductId1");

            migrationBuilder.RenameIndex(
                name: "IX_ReceiptDetails_ProductId",
                table: "AppReceiptDetails",
                newName: "IX_AppReceiptDetails_ProductId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AppReceipts",
                table: "AppReceipts",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AppReceiptDetails",
                table: "AppReceiptDetails",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_AppInventoryTransactions_LocationId",
                table: "AppInventoryTransactions",
                column: "LocationId");

            migrationBuilder.CreateIndex(
                name: "IX_AppInventoryTransactions_ProductId",
                table: "AppInventoryTransactions",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_AppInventoryTransactions_ReelId",
                table: "AppInventoryTransactions",
                column: "ReelId");

            migrationBuilder.AddForeignKey(
                name: "FK_AppInventoryTransactions_AppLocations_LocationId",
                table: "AppInventoryTransactions",
                column: "LocationId",
                principalTable: "AppLocations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AppInventoryTransactions_AppProducts_ProductId",
                table: "AppInventoryTransactions",
                column: "ProductId",
                principalTable: "AppProducts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AppInventoryTransactions_AppReels_ReelId",
                table: "AppInventoryTransactions",
                column: "ReelId",
                principalTable: "AppReels",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AppReceiptDetails_AppProducts_ProductId",
                table: "AppReceiptDetails",
                column: "ProductId",
                principalTable: "AppProducts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AppReceiptDetails_AppProducts_ProductId1",
                table: "AppReceiptDetails",
                column: "ProductId1",
                principalTable: "AppProducts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AppReceiptDetails_AppReceipts_ReceiptId",
                table: "AppReceiptDetails",
                column: "ReceiptId",
                principalTable: "AppReceipts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AppReceiptDetails_AppReels_ReelId",
                table: "AppReceiptDetails",
                column: "ReelId",
                principalTable: "AppReels",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AppReceiptDetails_AppReels_ReelId1",
                table: "AppReceiptDetails",
                column: "ReelId1",
                principalTable: "AppReels",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AppInventoryTransactions_AppLocations_LocationId",
                table: "AppInventoryTransactions");

            migrationBuilder.DropForeignKey(
                name: "FK_AppInventoryTransactions_AppProducts_ProductId",
                table: "AppInventoryTransactions");

            migrationBuilder.DropForeignKey(
                name: "FK_AppInventoryTransactions_AppReels_ReelId",
                table: "AppInventoryTransactions");

            migrationBuilder.DropForeignKey(
                name: "FK_AppReceiptDetails_AppProducts_ProductId",
                table: "AppReceiptDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_AppReceiptDetails_AppProducts_ProductId1",
                table: "AppReceiptDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_AppReceiptDetails_AppReceipts_ReceiptId",
                table: "AppReceiptDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_AppReceiptDetails_AppReels_ReelId",
                table: "AppReceiptDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_AppReceiptDetails_AppReels_ReelId1",
                table: "AppReceiptDetails");

            migrationBuilder.DropIndex(
                name: "IX_AppInventoryTransactions_LocationId",
                table: "AppInventoryTransactions");

            migrationBuilder.DropIndex(
                name: "IX_AppInventoryTransactions_ProductId",
                table: "AppInventoryTransactions");

            migrationBuilder.DropIndex(
                name: "IX_AppInventoryTransactions_ReelId",
                table: "AppInventoryTransactions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AppReceipts",
                table: "AppReceipts");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AppReceiptDetails",
                table: "AppReceiptDetails");

            migrationBuilder.RenameTable(
                name: "AppReceipts",
                newName: "Receipts");

            migrationBuilder.RenameTable(
                name: "AppReceiptDetails",
                newName: "ReceiptDetails");

            migrationBuilder.RenameIndex(
                name: "IX_AppReceipts_BillNo",
                table: "Receipts",
                newName: "IX_Receipts_BillNo");

            migrationBuilder.RenameIndex(
                name: "IX_AppReceiptDetails_ReelId1",
                table: "ReceiptDetails",
                newName: "IX_ReceiptDetails_ReelId1");

            migrationBuilder.RenameIndex(
                name: "IX_AppReceiptDetails_ReelId",
                table: "ReceiptDetails",
                newName: "IX_ReceiptDetails_ReelId");

            migrationBuilder.RenameIndex(
                name: "IX_AppReceiptDetails_ReceiptId",
                table: "ReceiptDetails",
                newName: "IX_ReceiptDetails_ReceiptId");

            migrationBuilder.RenameIndex(
                name: "IX_AppReceiptDetails_ProductId1",
                table: "ReceiptDetails",
                newName: "IX_ReceiptDetails_ProductId1");

            migrationBuilder.RenameIndex(
                name: "IX_AppReceiptDetails_ProductId",
                table: "ReceiptDetails",
                newName: "IX_ReceiptDetails_ProductId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Receipts",
                table: "Receipts",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ReceiptDetails",
                table: "ReceiptDetails",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ReceiptDetails_AppProducts_ProductId",
                table: "ReceiptDetails",
                column: "ProductId",
                principalTable: "AppProducts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ReceiptDetails_AppProducts_ProductId1",
                table: "ReceiptDetails",
                column: "ProductId1",
                principalTable: "AppProducts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ReceiptDetails_AppReels_ReelId",
                table: "ReceiptDetails",
                column: "ReelId",
                principalTable: "AppReels",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ReceiptDetails_AppReels_ReelId1",
                table: "ReceiptDetails",
                column: "ReelId1",
                principalTable: "AppReels",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ReceiptDetails_Receipts_ReceiptId",
                table: "ReceiptDetails",
                column: "ReceiptId",
                principalTable: "Receipts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}

