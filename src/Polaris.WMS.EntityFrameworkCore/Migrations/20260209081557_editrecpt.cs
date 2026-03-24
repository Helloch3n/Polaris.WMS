using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Polaris.WMS.Migrations
{
    /// <inheritdoc />
    public partial class editrecpt : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AppReceiptDetails_AppProducts_ProductId1",
                table: "AppReceiptDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_AppReceiptDetails_AppReels_ReelId1",
                table: "AppReceiptDetails");

            migrationBuilder.DropIndex(
                name: "IX_AppReceiptDetails_ProductId1",
                table: "AppReceiptDetails");

            migrationBuilder.DropIndex(
                name: "IX_AppReceiptDetails_ReelId1",
                table: "AppReceiptDetails");

            migrationBuilder.DropColumn(
                name: "ProductId1",
                table: "AppReceiptDetails");

            migrationBuilder.DropColumn(
                name: "ReelId1",
                table: "AppReceiptDetails");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "ProductId1",
                table: "AppReceiptDetails",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "ReelId1",
                table: "AppReceiptDetails",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_AppReceiptDetails_ProductId1",
                table: "AppReceiptDetails",
                column: "ProductId1");

            migrationBuilder.CreateIndex(
                name: "IX_AppReceiptDetails_ReelId1",
                table: "AppReceiptDetails",
                column: "ReelId1");

            migrationBuilder.AddForeignKey(
                name: "FK_AppReceiptDetails_AppProducts_ProductId1",
                table: "AppReceiptDetails",
                column: "ProductId1",
                principalTable: "AppProducts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AppReceiptDetails_AppReels_ReelId1",
                table: "AppReceiptDetails",
                column: "ReelId1",
                principalTable: "AppReels",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}

