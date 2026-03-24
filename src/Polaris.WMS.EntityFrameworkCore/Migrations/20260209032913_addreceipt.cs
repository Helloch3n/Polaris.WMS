using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Polaris.WMS.Migrations
{
    /// <inheritdoc />
    public partial class addreceipt : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Inventorys_AppProducts_ProductId",
                table: "Inventorys");

            migrationBuilder.DropForeignKey(
                name: "FK_Inventorys_AppReels_ReelId",
                table: "Inventorys");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Inventorys",
                table: "Inventorys");

            migrationBuilder.RenameTable(
                name: "Inventorys",
                newName: "AppInventorys");

            migrationBuilder.RenameIndex(
                name: "IX_Inventorys_ReelId",
                table: "AppInventorys",
                newName: "IX_AppInventorys_ReelId");

            migrationBuilder.RenameIndex(
                name: "IX_Inventorys_ProductId",
                table: "AppInventorys",
                newName: "IX_AppInventorys_ProductId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AppInventorys",
                table: "AppInventorys",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "AppInventoryTransactions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    BillNo = table.Column<string>(type: "text", nullable: false),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    InventoryId = table.Column<Guid>(type: "uuid", nullable: false),
                    ReelId = table.Column<Guid>(type: "uuid", nullable: false),
                    ProductId = table.Column<Guid>(type: "uuid", nullable: false),
                    LocationId = table.Column<Guid>(type: "uuid", nullable: false),
                    QuantityChange = table.Column<decimal>(type: "numeric", nullable: false),
                    QuantityAfter = table.Column<decimal>(type: "numeric", nullable: false),
                    BatchNo = table.Column<string>(type: "text", nullable: false),
                    CreationTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    CreatorId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppInventoryTransactions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Receipts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    BillNo = table.Column<string>(type: "text", nullable: false),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    SourceBillNo = table.Column<string>(type: "text", nullable: false),
                    WarehouseId = table.Column<Guid>(type: "uuid", nullable: false),
                    ExtraProperties = table.Column<string>(type: "text", nullable: false),
                    ConcurrencyStamp = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    CreationTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    CreatorId = table.Column<Guid>(type: "uuid", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    LastModifierId = table.Column<Guid>(type: "uuid", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    DeleterId = table.Column<Guid>(type: "uuid", nullable: true),
                    DeletionTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Receipts", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ReceiptDetails",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ReceiptId = table.Column<Guid>(type: "uuid", nullable: false),
                    ReelId = table.Column<Guid>(type: "uuid", nullable: false),
                    ProductId = table.Column<Guid>(type: "uuid", nullable: false),
                    PlanQuantity = table.Column<decimal>(type: "numeric", nullable: false),
                    ActualQuantity = table.Column<decimal>(type: "numeric", nullable: false),
                    IsReceived = table.Column<bool>(type: "boolean", nullable: false),
                    BatchNo = table.Column<string>(type: "text", nullable: false),
                    Source_WO = table.Column<string>(type: "text", nullable: false),
                    Layer_Index = table.Column<int>(type: "integer", nullable: false),
                    ReelId1 = table.Column<Guid>(type: "uuid", nullable: false),
                    ProductId1 = table.Column<Guid>(type: "uuid", nullable: false),
                    CreationTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    CreatorId = table.Column<Guid>(type: "uuid", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    LastModifierId = table.Column<Guid>(type: "uuid", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    DeleterId = table.Column<Guid>(type: "uuid", nullable: true),
                    DeletionTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReceiptDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ReceiptDetails_AppProducts_ProductId",
                        column: x => x.ProductId,
                        principalTable: "AppProducts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ReceiptDetails_AppProducts_ProductId1",
                        column: x => x.ProductId1,
                        principalTable: "AppProducts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ReceiptDetails_AppReels_ReelId",
                        column: x => x.ReelId,
                        principalTable: "AppReels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ReceiptDetails_AppReels_ReelId1",
                        column: x => x.ReelId1,
                        principalTable: "AppReels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ReceiptDetails_Receipts_ReceiptId",
                        column: x => x.ReceiptId,
                        principalTable: "Receipts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AppInventoryTransactions_BillNo",
                table: "AppInventoryTransactions",
                column: "BillNo");

            migrationBuilder.CreateIndex(
                name: "IX_AppInventoryTransactions_CreationTime",
                table: "AppInventoryTransactions",
                column: "CreationTime");

            migrationBuilder.CreateIndex(
                name: "IX_AppInventoryTransactions_InventoryId",
                table: "AppInventoryTransactions",
                column: "InventoryId");

            migrationBuilder.CreateIndex(
                name: "IX_ReceiptDetails_ProductId",
                table: "ReceiptDetails",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_ReceiptDetails_ProductId1",
                table: "ReceiptDetails",
                column: "ProductId1");

            migrationBuilder.CreateIndex(
                name: "IX_ReceiptDetails_ReceiptId",
                table: "ReceiptDetails",
                column: "ReceiptId");

            migrationBuilder.CreateIndex(
                name: "IX_ReceiptDetails_ReelId",
                table: "ReceiptDetails",
                column: "ReelId");

            migrationBuilder.CreateIndex(
                name: "IX_ReceiptDetails_ReelId1",
                table: "ReceiptDetails",
                column: "ReelId1");

            migrationBuilder.CreateIndex(
                name: "IX_Receipts_BillNo",
                table: "Receipts",
                column: "BillNo",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_AppInventorys_AppProducts_ProductId",
                table: "AppInventorys",
                column: "ProductId",
                principalTable: "AppProducts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AppInventorys_AppReels_ReelId",
                table: "AppInventorys",
                column: "ReelId",
                principalTable: "AppReels",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AppInventorys_AppProducts_ProductId",
                table: "AppInventorys");

            migrationBuilder.DropForeignKey(
                name: "FK_AppInventorys_AppReels_ReelId",
                table: "AppInventorys");

            migrationBuilder.DropTable(
                name: "AppInventoryTransactions");

            migrationBuilder.DropTable(
                name: "ReceiptDetails");

            migrationBuilder.DropTable(
                name: "Receipts");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AppInventorys",
                table: "AppInventorys");

            migrationBuilder.RenameTable(
                name: "AppInventorys",
                newName: "Inventorys");

            migrationBuilder.RenameIndex(
                name: "IX_AppInventorys_ReelId",
                table: "Inventorys",
                newName: "IX_Inventorys_ReelId");

            migrationBuilder.RenameIndex(
                name: "IX_AppInventorys_ProductId",
                table: "Inventorys",
                newName: "IX_Inventorys_ProductId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Inventorys",
                table: "Inventorys",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Inventorys_AppProducts_ProductId",
                table: "Inventorys",
                column: "ProductId",
                principalTable: "AppProducts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Inventorys_AppReels_ReelId",
                table: "Inventorys",
                column: "ReelId",
                principalTable: "AppReels",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}

