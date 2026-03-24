using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Polaris.WMS.Migrations
{
    /// <inheritdoc />
    public partial class _21001 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AvailableQuantity",
                table: "AppInventorys");

            migrationBuilder.AddColumn<string>(
                name: "SourceOrderNo",
                table: "AppOutboundOrders",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true,
                comment: "外部单号");

            migrationBuilder.AlterColumn<int>(
                name: "Quantity",
                table: "AppOutboundOrderItems",
                type: "integer",
                nullable: false,
                comment: "需求件数",
                oldClrType: typeof(decimal),
                oldType: "numeric(18,2)",
                oldPrecision: 18,
                oldScale: 2,
                oldComment: "需求数量");

            migrationBuilder.AddColumn<int>(
                name: "AllocatedQuantity",
                table: "AppOutboundOrderItems",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                comment: "已分配件数");

            migrationBuilder.AddColumn<decimal>(
                name: "TargetLength",
                table: "AppOutboundOrderItems",
                type: "numeric(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m,
                comment: "单根目标长度");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SourceOrderNo",
                table: "AppOutboundOrders");

            migrationBuilder.DropColumn(
                name: "AllocatedQuantity",
                table: "AppOutboundOrderItems");

            migrationBuilder.DropColumn(
                name: "TargetLength",
                table: "AppOutboundOrderItems");

            migrationBuilder.AlterColumn<decimal>(
                name: "Quantity",
                table: "AppOutboundOrderItems",
                type: "numeric(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                comment: "需求数量",
                oldClrType: typeof(int),
                oldType: "integer",
                oldComment: "需求件数");

            migrationBuilder.AddColumn<decimal>(
                name: "AvailableQuantity",
                table: "AppInventorys",
                type: "numeric(18,4)",
                precision: 18,
                scale: 4,
                nullable: false,
                defaultValue: 0m);
        }
    }
}

