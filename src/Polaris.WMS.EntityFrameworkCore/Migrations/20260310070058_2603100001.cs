using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Polaris.WMS.Migrations
{
    /// <inheritdoc />
    public partial class _2603100001 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_WmsProductionInboundDetails_ReelNo",
                table: "WmsProductionInboundDetails");

            migrationBuilder.DropColumn(
                name: "GrossWeight",
                table: "WmsProductionInboundDetails");

            migrationBuilder.DropColumn(
                name: "Length",
                table: "WmsProductionInboundDetails");

            migrationBuilder.DropColumn(
                name: "NetWeight",
                table: "WmsProductionInboundDetails");

            migrationBuilder.DropColumn(
                name: "ProductCode",
                table: "WmsProductionInboundDetails");

            migrationBuilder.DropColumn(
                name: "ProductName",
                table: "WmsProductionInboundDetails");

            migrationBuilder.RenameColumn(
                name: "ReelNo",
                table: "WmsProductionInboundDetails",
                newName: "RelatedOrderNoLineNo");

            migrationBuilder.RenameColumn(
                name: "PalletNo",
                table: "WmsProductionInboundDetails",
                newName: "RelatedOrderNo");

            migrationBuilder.AddColumn<string>(
                name: "CraftVersion",
                table: "WmsProductionInboundDetails",
                type: "character varying(64)",
                maxLength: 64,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "LayerIndex",
                table: "WmsProductionInboundDetails",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<Guid>(
                name: "ReelId",
                table: "WmsProductionInboundDetails",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<string>(
                name: "SN",
                table: "WmsProductionInboundDetails",
                type: "character varying(128)",
                maxLength: 128,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<decimal>(
                name: "Weight",
                table: "WmsProductionInboundDetails",
                type: "numeric(18,4)",
                precision: 18,
                scale: 4,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_WmsProductionInboundDetails_ReelId",
                table: "WmsProductionInboundDetails",
                column: "ReelId");

            migrationBuilder.CreateIndex(
                name: "IX_WmsProductionInboundDetails_SN",
                table: "WmsProductionInboundDetails",
                column: "SN");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_WmsProductionInboundDetails_ReelId",
                table: "WmsProductionInboundDetails");

            migrationBuilder.DropIndex(
                name: "IX_WmsProductionInboundDetails_SN",
                table: "WmsProductionInboundDetails");

            migrationBuilder.DropColumn(
                name: "CraftVersion",
                table: "WmsProductionInboundDetails");

            migrationBuilder.DropColumn(
                name: "LayerIndex",
                table: "WmsProductionInboundDetails");

            migrationBuilder.DropColumn(
                name: "ReelId",
                table: "WmsProductionInboundDetails");

            migrationBuilder.DropColumn(
                name: "SN",
                table: "WmsProductionInboundDetails");

            migrationBuilder.DropColumn(
                name: "Weight",
                table: "WmsProductionInboundDetails");

            migrationBuilder.RenameColumn(
                name: "RelatedOrderNoLineNo",
                table: "WmsProductionInboundDetails",
                newName: "ReelNo");

            migrationBuilder.RenameColumn(
                name: "RelatedOrderNo",
                table: "WmsProductionInboundDetails",
                newName: "PalletNo");

            migrationBuilder.AddColumn<decimal>(
                name: "GrossWeight",
                table: "WmsProductionInboundDetails",
                type: "numeric(18,4)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Length",
                table: "WmsProductionInboundDetails",
                type: "numeric(18,4)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "NetWeight",
                table: "WmsProductionInboundDetails",
                type: "numeric(18,4)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ProductCode",
                table: "WmsProductionInboundDetails",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ProductName",
                table: "WmsProductionInboundDetails",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_WmsProductionInboundDetails_ReelNo",
                table: "WmsProductionInboundDetails",
                column: "ReelNo");
        }
    }
}
