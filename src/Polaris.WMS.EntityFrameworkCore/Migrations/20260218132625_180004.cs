using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Polaris.WMS.Migrations
{
    /// <inheritdoc />
    public partial class _180004 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AppOutboundOrders",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    OrderNo = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false, comment: "出库单号"),
                    CustomerName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true, comment: "客户名称"),
                    Status = table.Column<int>(type: "integer", nullable: false, comment: "状态"),
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
                    table.PrimaryKey("PK_AppOutboundOrders", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AppPickTasks",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    OutboundOrderId = table.Column<Guid>(type: "uuid", nullable: false),
                    ReelNo = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false, comment: "盘具编号"),
                    FromLocation = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true, comment: "来源库位"),
                    ToLocation = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true, comment: "目标库位"),
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
                    table.PrimaryKey("PK_AppPickTasks", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AppOutboundOrderItems",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    OutboundOrderId = table.Column<Guid>(type: "uuid", nullable: false),
                    ProductCode = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false, comment: "物料编码"),
                    Quantity = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false, comment: "需求数量")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppOutboundOrderItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AppOutboundOrderItems_AppOutboundOrders_OutboundOrderId",
                        column: x => x.OutboundOrderId,
                        principalTable: "AppOutboundOrders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AppOutboundOrderItems_OutboundOrderId",
                table: "AppOutboundOrderItems",
                column: "OutboundOrderId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AppOutboundOrderItems");

            migrationBuilder.DropTable(
                name: "AppPickTasks");

            migrationBuilder.DropTable(
                name: "AppOutboundOrders");
        }
    }
}

