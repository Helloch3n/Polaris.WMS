using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Polaris.WMS.Migrations
{
    /// <inheritdoc />
    public partial class editreceipt122 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Source_WO",
                table: "AppReceiptDetails",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "BatchNo",
                table: "AppReceiptDetails",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddColumn<string>(
                name: "CraftVersion",
                table: "AppReceiptDetails",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SN",
                table: "AppReceiptDetails",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "AppReceiptDetails",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<Guid>(
                name: "ToWarehouseId",
                table: "AppReceiptDetails",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Weight",
                table: "AppReceiptDetails",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CraftVersion",
                table: "AppReceiptDetails");

            migrationBuilder.DropColumn(
                name: "SN",
                table: "AppReceiptDetails");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "AppReceiptDetails");

            migrationBuilder.DropColumn(
                name: "ToWarehouseId",
                table: "AppReceiptDetails");

            migrationBuilder.DropColumn(
                name: "Weight",
                table: "AppReceiptDetails");

            migrationBuilder.AlterColumn<string>(
                name: "Source_WO",
                table: "AppReceiptDetails",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "BatchNo",
                table: "AppReceiptDetails",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);
        }
    }
}

