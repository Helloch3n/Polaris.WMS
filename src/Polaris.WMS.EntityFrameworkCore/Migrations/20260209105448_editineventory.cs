using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Polaris.WMS.Migrations
{
    /// <inheritdoc />
    public partial class editineventory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Unit",
                table: "AppInventorys",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "Source_WO",
                table: "AppInventorys",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "BatchNo",
                table: "AppInventorys",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddColumn<decimal>(
                name: "AvailableQuantity",
                table: "AppInventorys",
                type: "numeric(18,4)",
                precision: 18,
                scale: 4,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "CraftVersion",
                table: "AppInventorys",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "LockedQuantity",
                table: "AppInventorys",
                type: "numeric(18,4)",
                precision: 18,
                scale: 4,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "SN",
                table: "AppInventorys",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            // Backfill existing rows: set SN to Id (unique) and AvailableQuantity to Quantity
            migrationBuilder.Sql(
                """UPDATE "AppInventorys" SET "SN" = "Id"::text WHERE "SN" = ''""");
            migrationBuilder.Sql(
                """UPDATE "AppInventorys" SET "AvailableQuantity" = "Quantity" WHERE "AvailableQuantity" = 0""");

            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "AppInventorys",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "Good");

            migrationBuilder.CreateIndex(
                name: "IX_AppInventorys_SN",
                table: "AppInventorys",
                column: "SN",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AppInventorys_Status",
                table: "AppInventorys",
                column: "Status");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_AppInventorys_SN",
                table: "AppInventorys");

            migrationBuilder.DropIndex(
                name: "IX_AppInventorys_Status",
                table: "AppInventorys");

            migrationBuilder.DropColumn(
                name: "AvailableQuantity",
                table: "AppInventorys");

            migrationBuilder.DropColumn(
                name: "CraftVersion",
                table: "AppInventorys");

            migrationBuilder.DropColumn(
                name: "LockedQuantity",
                table: "AppInventorys");

            migrationBuilder.DropColumn(
                name: "SN",
                table: "AppInventorys");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "AppInventorys");

            migrationBuilder.AlterColumn<string>(
                name: "Unit",
                table: "AppInventorys",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(20)",
                oldMaxLength: 20);

            migrationBuilder.AlterColumn<string>(
                name: "Source_WO",
                table: "AppInventorys",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<string>(
                name: "BatchNo",
                table: "AppInventorys",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(50)",
                oldMaxLength: 50);
        }
    }
}

