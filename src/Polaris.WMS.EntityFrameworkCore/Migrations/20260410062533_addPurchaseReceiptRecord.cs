using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Polaris.WMS.Migrations
{
    /// <inheritdoc />
    public partial class addPurchaseReceiptRecord : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_AppPurchaseReceiptDetails_ContainerId",
                table: "AppPurchaseReceiptDetails");

            migrationBuilder.DropIndex(
                name: "IX_AppPurchaseReceiptDetails_LocationId",
                table: "AppPurchaseReceiptDetails");

            migrationBuilder.DropColumn(
                name: "ContainerCode",
                table: "AppPurchaseReceiptDetails");

            migrationBuilder.DropColumn(
                name: "ContainerId",
                table: "AppPurchaseReceiptDetails");

            migrationBuilder.DropColumn(
                name: "LocationCode",
                table: "AppPurchaseReceiptDetails");

            migrationBuilder.DropColumn(
                name: "LocationId",
                table: "AppPurchaseReceiptDetails");

            migrationBuilder.AddColumn<decimal>(
                name: "ExpectedQuantity",
                table: "AppPurchaseReceiptDetails",
                type: "numeric(18,4)",
                precision: 18,
                scale: 4,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.CreateTable(
                name: "AppPurchaseRecords",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    PurchaseReceiptId = table.Column<Guid>(type: "uuid", nullable: false),
                    PurchaseReceiptDetailId = table.Column<Guid>(type: "uuid", nullable: false),
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
                    SupplierBatchNo = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppPurchaseRecords", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AppPurchaseRecords_AppPurchaseReceiptDetails_PurchaseReceip~",
                        column: x => x.PurchaseReceiptDetailId,
                        principalTable: "AppPurchaseReceiptDetails",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AppPurchaseRecords_ContainerId",
                table: "AppPurchaseRecords",
                column: "ContainerId");

            migrationBuilder.CreateIndex(
                name: "IX_AppPurchaseRecords_LocationId",
                table: "AppPurchaseRecords",
                column: "LocationId");

            migrationBuilder.CreateIndex(
                name: "IX_AppPurchaseRecords_ProductId",
                table: "AppPurchaseRecords",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_AppPurchaseRecords_PurchaseReceiptDetailId",
                table: "AppPurchaseRecords",
                column: "PurchaseReceiptDetailId");

            migrationBuilder.CreateIndex(
                name: "IX_AppPurchaseRecords_PurchaseReceiptId",
                table: "AppPurchaseRecords",
                column: "PurchaseReceiptId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AppPurchaseRecords");

            migrationBuilder.DropColumn(
                name: "ExpectedQuantity",
                table: "AppPurchaseReceiptDetails");

            migrationBuilder.AddColumn<string>(
                name: "ContainerCode",
                table: "AppPurchaseReceiptDetails",
                type: "character varying(64)",
                maxLength: 64,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<Guid>(
                name: "ContainerId",
                table: "AppPurchaseReceiptDetails",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<string>(
                name: "LocationCode",
                table: "AppPurchaseReceiptDetails",
                type: "character varying(64)",
                maxLength: 64,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<Guid>(
                name: "LocationId",
                table: "AppPurchaseReceiptDetails",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_AppPurchaseReceiptDetails_ContainerId",
                table: "AppPurchaseReceiptDetails",
                column: "ContainerId");

            migrationBuilder.CreateIndex(
                name: "IX_AppPurchaseReceiptDetails_LocationId",
                table: "AppPurchaseReceiptDetails",
                column: "LocationId");
        }
    }
}
