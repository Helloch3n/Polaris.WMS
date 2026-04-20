using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Polaris.WMS.Migrations
{
    /// <inheritdoc />
    public partial class addmixinbound : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AppCycleCountOrders",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: true),
                    WarehouseId = table.Column<Guid>(type: "uuid", nullable: false),
                    OrderNo = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    CountType = table.Column<int>(type: "integer", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
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
                    table.PrimaryKey("PK_AppCycleCountOrders", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AppMiscInboundOrders",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    OrderNo = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
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
                    table.PrimaryKey("PK_AppMiscInboundOrders", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AppMiscOutboundOrders",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    OrderNo = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
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
                    table.PrimaryKey("PK_AppMiscOutboundOrders", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AppCycleCountOrderDetails",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CycleCountOrderId = table.Column<Guid>(type: "uuid", nullable: false),
                    LocationId = table.Column<Guid>(type: "uuid", nullable: false),
                    ContainerCode = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    ProductId = table.Column<Guid>(type: "uuid", nullable: false),
                    SystemQty = table.Column<decimal>(type: "numeric(18,4)", precision: 18, scale: 4, nullable: false),
                    CountedQty = table.Column<decimal>(type: "numeric(18,4)", precision: 18, scale: 4, nullable: true),
                    DifferenceQty = table.Column<decimal>(type: "numeric(18,4)", precision: 18, scale: 4, nullable: false),
                    IsCounted = table.Column<bool>(type: "boolean", nullable: false),
                    CountedTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    IsInventoryAdjusted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppCycleCountOrderDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AppCycleCountOrderDetails_AppCycleCountOrders_CycleCountOrd~",
                        column: x => x.CycleCountOrderId,
                        principalTable: "AppCycleCountOrders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AppMiscInboundOrderDetails",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    MiscInboundOrderId = table.Column<Guid>(type: "uuid", nullable: false),
                    ContainerCode = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    ProductId = table.Column<Guid>(type: "uuid", nullable: false),
                    Qty = table.Column<decimal>(type: "numeric(18,4)", precision: 18, scale: 4, nullable: false),
                    Remark = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppMiscInboundOrderDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AppMiscInboundOrderDetails_AppMiscInboundOrders_MiscInbound~",
                        column: x => x.MiscInboundOrderId,
                        principalTable: "AppMiscInboundOrders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AppMiscOutboundOrderDetails",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    MiscOutboundOrderId = table.Column<Guid>(type: "uuid", nullable: false),
                    ContainerCode = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    ProductId = table.Column<Guid>(type: "uuid", nullable: false),
                    Qty = table.Column<decimal>(type: "numeric(18,4)", precision: 18, scale: 4, nullable: false),
                    Remark = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppMiscOutboundOrderDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AppMiscOutboundOrderDetails_AppMiscOutboundOrders_MiscOutbo~",
                        column: x => x.MiscOutboundOrderId,
                        principalTable: "AppMiscOutboundOrders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AppCycleCountOrderDetails_ContainerCode_ProductId",
                table: "AppCycleCountOrderDetails",
                columns: new[] { "ContainerCode", "ProductId" });

            migrationBuilder.CreateIndex(
                name: "IX_AppCycleCountOrderDetails_CycleCountOrderId",
                table: "AppCycleCountOrderDetails",
                column: "CycleCountOrderId");

            migrationBuilder.CreateIndex(
                name: "IX_AppCycleCountOrderDetails_LocationId",
                table: "AppCycleCountOrderDetails",
                column: "LocationId");

            migrationBuilder.CreateIndex(
                name: "IX_AppCycleCountOrders_OrderNo",
                table: "AppCycleCountOrders",
                column: "OrderNo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AppCycleCountOrders_WarehouseId",
                table: "AppCycleCountOrders",
                column: "WarehouseId");

            migrationBuilder.CreateIndex(
                name: "IX_AppMiscInboundOrderDetails_MiscInboundOrderId",
                table: "AppMiscInboundOrderDetails",
                column: "MiscInboundOrderId");

            migrationBuilder.CreateIndex(
                name: "IX_AppMiscInboundOrderDetails_ProductId",
                table: "AppMiscInboundOrderDetails",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_AppMiscInboundOrders_OrderNo",
                table: "AppMiscInboundOrders",
                column: "OrderNo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AppMiscOutboundOrderDetails_MiscOutboundOrderId",
                table: "AppMiscOutboundOrderDetails",
                column: "MiscOutboundOrderId");

            migrationBuilder.CreateIndex(
                name: "IX_AppMiscOutboundOrderDetails_ProductId",
                table: "AppMiscOutboundOrderDetails",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_AppMiscOutboundOrders_OrderNo",
                table: "AppMiscOutboundOrders",
                column: "OrderNo",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AppCycleCountOrderDetails");

            migrationBuilder.DropTable(
                name: "AppMiscInboundOrderDetails");

            migrationBuilder.DropTable(
                name: "AppMiscOutboundOrderDetails");

            migrationBuilder.DropTable(
                name: "AppCycleCountOrders");

            migrationBuilder.DropTable(
                name: "AppMiscInboundOrders");

            migrationBuilder.DropTable(
                name: "AppMiscOutboundOrders");
        }
    }
}
