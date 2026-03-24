using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Polaris.WMS.Migrations
{
    /// <inheritdoc />
    public partial class _2603010002 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AppInventorys_AppProducts_ProductId",
                table: "AppInventorys");

            migrationBuilder.DropForeignKey(
                name: "FK_AppInventorys_AppReels_ReelId",
                table: "AppInventorys");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddForeignKey(
                name: "FK_AppInventorys_AppProducts_ProductId",
                table: "AppInventorys",
                column: "ProductId",
                principalTable: "AppProducts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AppInventorys_AppReels_ReelId",
                table: "AppInventorys",
                column: "ReelId",
                principalTable: "AppReels",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
