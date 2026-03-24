using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Polaris.WMS.Migrations
{
    /// <inheritdoc />
    public partial class _202603060002 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<Guid>(
                name: "WarehouseId",
                table: "AppTransferOrders",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "TargetWarehouseId",
                table: "AppTransferOrders",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "SourceWarehouseId",
                table: "AppTransferOrders",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "SourceDepartmentId",
                table: "AppTransferOrders",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "DepartmentId",
                table: "AppTransferOrders",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.CreateTable(
                name: "WmsProductionInbounds",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    OrderNo = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    SourceOrderNo = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    SourceDepartmentId = table.Column<Guid>(type: "uuid", nullable: false),
                    TargetWarehouseId = table.Column<Guid>(type: "uuid", nullable: false),
                    WarehouseId = table.Column<Guid>(type: "uuid", nullable: false),
                    DepartmentId = table.Column<Guid>(type: "uuid", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    InboundType = table.Column<int>(type: "integer", nullable: false),
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
                    table.PrimaryKey("PK_WmsProductionInbounds", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "WmsProductionInboundDetails",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ProductionInboundId = table.Column<Guid>(type: "uuid", nullable: false),
                    ProductId = table.Column<Guid>(type: "uuid", nullable: false),
                    ProductCode = table.Column<string>(type: "text", nullable: false),
                    ProductName = table.Column<string>(type: "text", nullable: false),
                    BatchNo = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    ReelNo = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    PalletNo = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    Qty = table.Column<decimal>(type: "numeric(18,4)", nullable: false),
                    Unit = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: false),
                    Length = table.Column<decimal>(type: "numeric(18,4)", nullable: true),
                    NetWeight = table.Column<decimal>(type: "numeric(18,4)", nullable: true),
                    GrossWeight = table.Column<decimal>(type: "numeric(18,4)", nullable: true),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    ActualLocationId = table.Column<Guid>(type: "uuid", nullable: true),
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
                    table.PrimaryKey("PK_WmsProductionInboundDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WmsProductionInboundDetails_WmsProductionInbounds_Productio~",
                        column: x => x.ProductionInboundId,
                        principalTable: "WmsProductionInbounds",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_WmsProductionInboundDetails_BatchNo",
                table: "WmsProductionInboundDetails",
                column: "BatchNo");

            migrationBuilder.CreateIndex(
                name: "IX_WmsProductionInboundDetails_ProductionInboundId",
                table: "WmsProductionInboundDetails",
                column: "ProductionInboundId");

            migrationBuilder.CreateIndex(
                name: "IX_WmsProductionInboundDetails_ReelNo",
                table: "WmsProductionInboundDetails",
                column: "ReelNo");

            migrationBuilder.CreateIndex(
                name: "IX_WmsProductionInbounds_DepartmentId",
                table: "WmsProductionInbounds",
                column: "DepartmentId");

            migrationBuilder.CreateIndex(
                name: "IX_WmsProductionInbounds_OrderNo",
                table: "WmsProductionInbounds",
                column: "OrderNo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_WmsProductionInbounds_SourceOrderNo",
                table: "WmsProductionInbounds",
                column: "SourceOrderNo");

            migrationBuilder.CreateIndex(
                name: "IX_WmsProductionInbounds_WarehouseId",
                table: "WmsProductionInbounds",
                column: "WarehouseId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "WmsProductionInboundDetails");

            migrationBuilder.DropTable(
                name: "WmsProductionInbounds");

            migrationBuilder.AlterColumn<Guid>(
                name: "WarehouseId",
                table: "AppTransferOrders",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AlterColumn<Guid>(
                name: "TargetWarehouseId",
                table: "AppTransferOrders",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AlterColumn<Guid>(
                name: "SourceWarehouseId",
                table: "AppTransferOrders",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AlterColumn<Guid>(
                name: "SourceDepartmentId",
                table: "AppTransferOrders",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AlterColumn<Guid>(
                name: "DepartmentId",
                table: "AppTransferOrders",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");
        }
    }
}
