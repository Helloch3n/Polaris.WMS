using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Polaris.WMS.Migrations
{
    /// <inheritdoc />
    public partial class _2603020001 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Type",
                table: "AppInventorys",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "SemiFinished");

            migrationBuilder.CreateIndex(
                name: "IX_AppInventorys_Type",
                table: "AppInventorys",
                column: "Type");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_AppInventorys_Type",
                table: "AppInventorys");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "AppInventorys");
        }
    }
}
