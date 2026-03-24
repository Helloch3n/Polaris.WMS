using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Polaris.WMS.Migrations
{
    /// <inheritdoc />
    public partial class editlocationindx : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_AppLocations_Code",
                table: "AppLocations");

            migrationBuilder.DropIndex(
                name: "IX_AppLocations_ZoneId",
                table: "AppLocations");

            migrationBuilder.CreateIndex(
                name: "IX_AppLocations_ZoneId_Code",
                table: "AppLocations",
                columns: new[] { "ZoneId", "Code" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_AppLocations_ZoneId_Code",
                table: "AppLocations");

            migrationBuilder.CreateIndex(
                name: "IX_AppLocations_Code",
                table: "AppLocations",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AppLocations_ZoneId",
                table: "AppLocations",
                column: "ZoneId");
        }
    }
}

