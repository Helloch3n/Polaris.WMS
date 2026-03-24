using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Polaris.WMS.Migrations
{
    /// <inheritdoc />
    public partial class _2602130003 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "AppLocations",
                type: "text",
                nullable: false,
                comment: "库位状态",
                oldClrType: typeof(int),
                oldType: "integer",
                oldComment: "库位状态");

            migrationBuilder.AddColumn<bool>(
                name: "AllowMixedBatches",
                table: "AppLocations",
                type: "boolean",
                nullable: false,
                defaultValue: false,
                comment: "是否允许混放不同批次");

            migrationBuilder.AddColumn<bool>(
                name: "AllowMixedProducts",
                table: "AppLocations",
                type: "boolean",
                nullable: false,
                defaultValue: false,
                comment: "是否允许混放不同物料");

            migrationBuilder.AddColumn<int>(
                name: "MaxReelCount",
                table: "AppLocations",
                type: "integer",
                nullable: false,
                defaultValue: 1,
                comment: "最大盘数");

            migrationBuilder.AddColumn<string>(
                name: "Type",
                table: "AppLocations",
                type: "text",
                nullable: false,
                defaultValue: "",
                comment: "库位类型");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AllowMixedBatches",
                table: "AppLocations");

            migrationBuilder.DropColumn(
                name: "AllowMixedProducts",
                table: "AppLocations");

            migrationBuilder.DropColumn(
                name: "MaxReelCount",
                table: "AppLocations");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "AppLocations");

            migrationBuilder.AlterColumn<int>(
                name: "Status",
                table: "AppLocations",
                type: "integer",
                nullable: false,
                comment: "库位状态",
                oldClrType: typeof(string),
                oldType: "text",
                oldComment: "库位状态");
        }
    }
}

