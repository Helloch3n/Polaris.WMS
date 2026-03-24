using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Polaris.WMS.Migrations
{
    /// <inheritdoc />
    public partial class _260212001 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SourceBillNo",
                table: "AppReceipts");

            migrationBuilder.AddColumn<string>(
                name: "Unit",
                table: "AppReceiptDetails",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Unit",
                table: "AppReceiptDetails");

            migrationBuilder.AddColumn<string>(
                name: "SourceBillNo",
                table: "AppReceipts",
                type: "text",
                nullable: false,
                defaultValue: "");
        }
    }
}

