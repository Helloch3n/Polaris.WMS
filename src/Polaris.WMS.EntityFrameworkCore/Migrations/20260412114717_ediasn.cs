using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Polaris.WMS.Migrations
{
    /// <inheritdoc />
    public partial class ediasn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "SourceAsnLineId",
                table: "AppPurchaseReceiptDetails",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "SourcePoLineId",
                table: "AppPurchaseReceiptDetails",
                type: "uuid",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SourceAsnLineId",
                table: "AppPurchaseReceiptDetails");

            migrationBuilder.DropColumn(
                name: "SourcePoLineId",
                table: "AppPurchaseReceiptDetails");
        }
    }
}
