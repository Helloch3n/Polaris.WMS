using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Polaris.WMS.Migrations
{
    /// <inheritdoc />
    public partial class _250002 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Height",
                table: "AppProducts");

            migrationBuilder.DropColumn(
                name: "Length",
                table: "AppProducts");

            migrationBuilder.DropColumn(
                name: "Weight",
                table: "AppProducts");

            migrationBuilder.DropColumn(
                name: "Width",
                table: "AppProducts");

            migrationBuilder.AddColumn<string>(
                name: "AuxUnit",
                table: "AppProducts",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "",
                comment: "辅助单位");

            migrationBuilder.AddColumn<decimal>(
                name: "ConversionRate",
                table: "AppProducts",
                type: "numeric",
                nullable: false,
                defaultValue: 0m,
                comment: "转换比例");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AuxUnit",
                table: "AppProducts");

            migrationBuilder.DropColumn(
                name: "ConversionRate",
                table: "AppProducts");

            migrationBuilder.AddColumn<decimal>(
                name: "Height",
                table: "AppProducts",
                type: "numeric(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m,
                comment: "高度(cm)");

            migrationBuilder.AddColumn<decimal>(
                name: "Length",
                table: "AppProducts",
                type: "numeric(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m,
                comment: "长度(cm)");

            migrationBuilder.AddColumn<decimal>(
                name: "Weight",
                table: "AppProducts",
                type: "numeric(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m,
                comment: "重量(kg)");

            migrationBuilder.AddColumn<decimal>(
                name: "Width",
                table: "AppProducts",
                type: "numeric(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m,
                comment: "宽度(cm)");
        }
    }
}

