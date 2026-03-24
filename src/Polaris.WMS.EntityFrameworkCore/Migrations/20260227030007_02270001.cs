using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Polaris.WMS.Migrations
{
    /// <inheritdoc />
    public partial class _02270001 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MaxWeight",
                table: "AppReels");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "AppReels");

            migrationBuilder.AddColumn<string>(
                name: "Size",
                table: "AppReels",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Size",
                table: "AppReels");

            migrationBuilder.AddColumn<decimal>(
                name: "MaxWeight",
                table: "AppReels",
                type: "numeric(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "Type",
                table: "AppReels",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }
    }
}

