using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Polaris.WMS.Migrations
{
    /// <inheritdoc />
    public partial class editmixinbound : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AccountAliasDescription",
                table: "AppMiscOutboundOrders",
                type: "character varying(500)",
                maxLength: 500,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<Guid>(
                name: "AccountAliasId",
                table: "AppMiscOutboundOrders",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<string>(
                name: "CostCenterCode",
                table: "AppMiscOutboundOrders",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<Guid>(
                name: "CostCenterId",
                table: "AppMiscOutboundOrders",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<string>(
                name: "CostCenterName",
                table: "AppMiscOutboundOrders",
                type: "character varying(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "BatchNo",
                table: "AppMiscOutboundOrderDetails",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<Guid>(
                name: "ContainerId",
                table: "AppMiscOutboundOrderDetails",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<string>(
                name: "CraftVersion",
                table: "AppMiscOutboundOrderDetails",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LocationCode",
                table: "AppMiscOutboundOrderDetails",
                type: "character varying(64)",
                maxLength: 64,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<Guid>(
                name: "LocationId",
                table: "AppMiscOutboundOrderDetails",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<string>(
                name: "ProductCode",
                table: "AppMiscOutboundOrderDetails",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ProductName",
                table: "AppMiscOutboundOrderDetails",
                type: "character varying(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "SN",
                table: "AppMiscOutboundOrderDetails",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Unit",
                table: "AppMiscOutboundOrderDetails",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "WarehouseCode",
                table: "AppMiscOutboundOrderDetails",
                type: "character varying(64)",
                maxLength: 64,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<Guid>(
                name: "WarehouseId",
                table: "AppMiscOutboundOrderDetails",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<string>(
                name: "WarehouseName",
                table: "AppMiscOutboundOrderDetails",
                type: "character varying(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "AccountAliasDescription",
                table: "AppMiscInboundOrders",
                type: "character varying(500)",
                maxLength: 500,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<Guid>(
                name: "AccountAliasId",
                table: "AppMiscInboundOrders",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<string>(
                name: "CostCenterCode",
                table: "AppMiscInboundOrders",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<Guid>(
                name: "CostCenterId",
                table: "AppMiscInboundOrders",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<string>(
                name: "CostCenterName",
                table: "AppMiscInboundOrders",
                type: "character varying(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "BatchNo",
                table: "AppMiscInboundOrderDetails",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<Guid>(
                name: "ContainerId",
                table: "AppMiscInboundOrderDetails",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<string>(
                name: "CraftVersion",
                table: "AppMiscInboundOrderDetails",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LocationCode",
                table: "AppMiscInboundOrderDetails",
                type: "character varying(64)",
                maxLength: 64,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<Guid>(
                name: "LocationId",
                table: "AppMiscInboundOrderDetails",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<string>(
                name: "ProductCode",
                table: "AppMiscInboundOrderDetails",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ProductName",
                table: "AppMiscInboundOrderDetails",
                type: "character varying(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "SN",
                table: "AppMiscInboundOrderDetails",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Unit",
                table: "AppMiscInboundOrderDetails",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "WarehouseCode",
                table: "AppMiscInboundOrderDetails",
                type: "character varying(64)",
                maxLength: 64,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<Guid>(
                name: "WarehouseId",
                table: "AppMiscInboundOrderDetails",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<string>(
                name: "WarehouseName",
                table: "AppMiscInboundOrderDetails",
                type: "character varying(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_AppMiscOutboundOrders_AccountAliasId",
                table: "AppMiscOutboundOrders",
                column: "AccountAliasId");

            migrationBuilder.CreateIndex(
                name: "IX_AppMiscOutboundOrders_CostCenterId",
                table: "AppMiscOutboundOrders",
                column: "CostCenterId");

            migrationBuilder.CreateIndex(
                name: "IX_AppMiscOutboundOrderDetails_ContainerId",
                table: "AppMiscOutboundOrderDetails",
                column: "ContainerId");

            migrationBuilder.CreateIndex(
                name: "IX_AppMiscOutboundOrderDetails_LocationId",
                table: "AppMiscOutboundOrderDetails",
                column: "LocationId");

            migrationBuilder.CreateIndex(
                name: "IX_AppMiscOutboundOrderDetails_SN",
                table: "AppMiscOutboundOrderDetails",
                column: "SN");

            migrationBuilder.CreateIndex(
                name: "IX_AppMiscOutboundOrderDetails_WarehouseId",
                table: "AppMiscOutboundOrderDetails",
                column: "WarehouseId");

            migrationBuilder.CreateIndex(
                name: "IX_AppMiscInboundOrders_AccountAliasId",
                table: "AppMiscInboundOrders",
                column: "AccountAliasId");

            migrationBuilder.CreateIndex(
                name: "IX_AppMiscInboundOrders_CostCenterId",
                table: "AppMiscInboundOrders",
                column: "CostCenterId");

            migrationBuilder.CreateIndex(
                name: "IX_AppMiscInboundOrderDetails_ContainerId",
                table: "AppMiscInboundOrderDetails",
                column: "ContainerId");

            migrationBuilder.CreateIndex(
                name: "IX_AppMiscInboundOrderDetails_LocationId",
                table: "AppMiscInboundOrderDetails",
                column: "LocationId");

            migrationBuilder.CreateIndex(
                name: "IX_AppMiscInboundOrderDetails_SN",
                table: "AppMiscInboundOrderDetails",
                column: "SN");

            migrationBuilder.CreateIndex(
                name: "IX_AppMiscInboundOrderDetails_WarehouseId",
                table: "AppMiscInboundOrderDetails",
                column: "WarehouseId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_AppMiscOutboundOrders_AccountAliasId",
                table: "AppMiscOutboundOrders");

            migrationBuilder.DropIndex(
                name: "IX_AppMiscOutboundOrders_CostCenterId",
                table: "AppMiscOutboundOrders");

            migrationBuilder.DropIndex(
                name: "IX_AppMiscOutboundOrderDetails_ContainerId",
                table: "AppMiscOutboundOrderDetails");

            migrationBuilder.DropIndex(
                name: "IX_AppMiscOutboundOrderDetails_LocationId",
                table: "AppMiscOutboundOrderDetails");

            migrationBuilder.DropIndex(
                name: "IX_AppMiscOutboundOrderDetails_SN",
                table: "AppMiscOutboundOrderDetails");

            migrationBuilder.DropIndex(
                name: "IX_AppMiscOutboundOrderDetails_WarehouseId",
                table: "AppMiscOutboundOrderDetails");

            migrationBuilder.DropIndex(
                name: "IX_AppMiscInboundOrders_AccountAliasId",
                table: "AppMiscInboundOrders");

            migrationBuilder.DropIndex(
                name: "IX_AppMiscInboundOrders_CostCenterId",
                table: "AppMiscInboundOrders");

            migrationBuilder.DropIndex(
                name: "IX_AppMiscInboundOrderDetails_ContainerId",
                table: "AppMiscInboundOrderDetails");

            migrationBuilder.DropIndex(
                name: "IX_AppMiscInboundOrderDetails_LocationId",
                table: "AppMiscInboundOrderDetails");

            migrationBuilder.DropIndex(
                name: "IX_AppMiscInboundOrderDetails_SN",
                table: "AppMiscInboundOrderDetails");

            migrationBuilder.DropIndex(
                name: "IX_AppMiscInboundOrderDetails_WarehouseId",
                table: "AppMiscInboundOrderDetails");

            migrationBuilder.DropColumn(
                name: "AccountAliasDescription",
                table: "AppMiscOutboundOrders");

            migrationBuilder.DropColumn(
                name: "AccountAliasId",
                table: "AppMiscOutboundOrders");

            migrationBuilder.DropColumn(
                name: "CostCenterCode",
                table: "AppMiscOutboundOrders");

            migrationBuilder.DropColumn(
                name: "CostCenterId",
                table: "AppMiscOutboundOrders");

            migrationBuilder.DropColumn(
                name: "CostCenterName",
                table: "AppMiscOutboundOrders");

            migrationBuilder.DropColumn(
                name: "BatchNo",
                table: "AppMiscOutboundOrderDetails");

            migrationBuilder.DropColumn(
                name: "ContainerId",
                table: "AppMiscOutboundOrderDetails");

            migrationBuilder.DropColumn(
                name: "CraftVersion",
                table: "AppMiscOutboundOrderDetails");

            migrationBuilder.DropColumn(
                name: "LocationCode",
                table: "AppMiscOutboundOrderDetails");

            migrationBuilder.DropColumn(
                name: "LocationId",
                table: "AppMiscOutboundOrderDetails");

            migrationBuilder.DropColumn(
                name: "ProductCode",
                table: "AppMiscOutboundOrderDetails");

            migrationBuilder.DropColumn(
                name: "ProductName",
                table: "AppMiscOutboundOrderDetails");

            migrationBuilder.DropColumn(
                name: "SN",
                table: "AppMiscOutboundOrderDetails");

            migrationBuilder.DropColumn(
                name: "Unit",
                table: "AppMiscOutboundOrderDetails");

            migrationBuilder.DropColumn(
                name: "WarehouseCode",
                table: "AppMiscOutboundOrderDetails");

            migrationBuilder.DropColumn(
                name: "WarehouseId",
                table: "AppMiscOutboundOrderDetails");

            migrationBuilder.DropColumn(
                name: "WarehouseName",
                table: "AppMiscOutboundOrderDetails");

            migrationBuilder.DropColumn(
                name: "AccountAliasDescription",
                table: "AppMiscInboundOrders");

            migrationBuilder.DropColumn(
                name: "AccountAliasId",
                table: "AppMiscInboundOrders");

            migrationBuilder.DropColumn(
                name: "CostCenterCode",
                table: "AppMiscInboundOrders");

            migrationBuilder.DropColumn(
                name: "CostCenterId",
                table: "AppMiscInboundOrders");

            migrationBuilder.DropColumn(
                name: "CostCenterName",
                table: "AppMiscInboundOrders");

            migrationBuilder.DropColumn(
                name: "BatchNo",
                table: "AppMiscInboundOrderDetails");

            migrationBuilder.DropColumn(
                name: "ContainerId",
                table: "AppMiscInboundOrderDetails");

            migrationBuilder.DropColumn(
                name: "CraftVersion",
                table: "AppMiscInboundOrderDetails");

            migrationBuilder.DropColumn(
                name: "LocationCode",
                table: "AppMiscInboundOrderDetails");

            migrationBuilder.DropColumn(
                name: "LocationId",
                table: "AppMiscInboundOrderDetails");

            migrationBuilder.DropColumn(
                name: "ProductCode",
                table: "AppMiscInboundOrderDetails");

            migrationBuilder.DropColumn(
                name: "ProductName",
                table: "AppMiscInboundOrderDetails");

            migrationBuilder.DropColumn(
                name: "SN",
                table: "AppMiscInboundOrderDetails");

            migrationBuilder.DropColumn(
                name: "Unit",
                table: "AppMiscInboundOrderDetails");

            migrationBuilder.DropColumn(
                name: "WarehouseCode",
                table: "AppMiscInboundOrderDetails");

            migrationBuilder.DropColumn(
                name: "WarehouseId",
                table: "AppMiscInboundOrderDetails");

            migrationBuilder.DropColumn(
                name: "WarehouseName",
                table: "AppMiscInboundOrderDetails");
        }
    }
}
