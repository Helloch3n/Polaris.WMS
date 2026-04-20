using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Polaris.WMS.Migrations
{
    /// <inheritdoc />
    public partial class ChangePurchaseReceiptLocationCodeToString : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AppPurchaseReceipts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ReceiptNo = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    SourceDocType = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    SourceDocNo = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    SupplierId = table.Column<Guid>(type: "uuid", nullable: true),
                    SupplierName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    Remark = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
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
                    table.PrimaryKey("PK_AppPurchaseReceipts", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AppPurchaseReceiptDetails",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    PurchaseReceiptId = table.Column<Guid>(type: "uuid", nullable: false),
                    SourceDetailId = table.Column<Guid>(type: "uuid", nullable: true),
                    ProductId = table.Column<Guid>(type: "uuid", nullable: false),
                    ProductName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    ProductCode = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    ReceivedQuantity = table.Column<decimal>(type: "numeric(18,4)", precision: 18, scale: 4, nullable: false),
                    ContainerId = table.Column<Guid>(type: "uuid", nullable: false),
                    ContainerCode = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    LocationId = table.Column<Guid>(type: "uuid", nullable: false),
                    LocationCode = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    BatchNo = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    ErpSyncStatus = table.Column<int>(type: "integer", nullable: false),
                    ErpSyncErrorMessage = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppPurchaseReceiptDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AppPurchaseReceiptDetails_AppPurchaseReceipts_PurchaseRecei~",
                        column: x => x.PurchaseReceiptId,
                        principalTable: "AppPurchaseReceipts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AppPurchaseReceiptDetails_ContainerId",
                table: "AppPurchaseReceiptDetails",
                column: "ContainerId");

            migrationBuilder.CreateIndex(
                name: "IX_AppPurchaseReceiptDetails_ErpSyncStatus",
                table: "AppPurchaseReceiptDetails",
                column: "ErpSyncStatus");

            migrationBuilder.CreateIndex(
                name: "IX_AppPurchaseReceiptDetails_LocationId",
                table: "AppPurchaseReceiptDetails",
                column: "LocationId");

            migrationBuilder.CreateIndex(
                name: "IX_AppPurchaseReceiptDetails_ProductId",
                table: "AppPurchaseReceiptDetails",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_AppPurchaseReceiptDetails_PurchaseReceiptId",
                table: "AppPurchaseReceiptDetails",
                column: "PurchaseReceiptId");

            migrationBuilder.CreateIndex(
                name: "IX_AppPurchaseReceipts_ReceiptNo",
                table: "AppPurchaseReceipts",
                column: "ReceiptNo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AppPurchaseReceipts_SourceDocType_SourceDocNo",
                table: "AppPurchaseReceipts",
                columns: new[] { "SourceDocType", "SourceDocNo" });

            migrationBuilder.CreateIndex(
                name: "IX_AppPurchaseReceipts_SupplierId",
                table: "AppPurchaseReceipts",
                column: "SupplierId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AppPurchaseReceiptDetails");

            migrationBuilder.DropTable(
                name: "AppPurchaseReceipts");

        }
    }
}
