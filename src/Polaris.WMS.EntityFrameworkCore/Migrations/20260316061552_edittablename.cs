using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Polaris.WMS.Migrations
{
    /// <inheritdoc />
    public partial class edittablename : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WmsDataSyncFieldMappings_WmsDataSyncTasks_TaskId",
                table: "WmsDataSyncFieldMappings");

            migrationBuilder.DropForeignKey(
                name: "FK_WmsProductionInboundDetails_WmsProductionInbounds_Productio~",
                table: "WmsProductionInboundDetails");

            migrationBuilder.DropPrimaryKey(
                name: "PK_WmsProductionInbounds",
                table: "WmsProductionInbounds");

            migrationBuilder.DropPrimaryKey(
                name: "PK_WmsProductionInboundDetails",
                table: "WmsProductionInboundDetails");

            migrationBuilder.DropPrimaryKey(
                name: "PK_WmsDataSyncTasks",
                table: "WmsDataSyncTasks");

            migrationBuilder.DropPrimaryKey(
                name: "PK_WmsDataSyncFieldMappings",
                table: "WmsDataSyncFieldMappings");

            migrationBuilder.RenameTable(
                name: "WmsProductionInbounds",
                newName: "AppProductionInbounds");

            migrationBuilder.RenameTable(
                name: "WmsProductionInboundDetails",
                newName: "AppProductionInboundDetails");

            migrationBuilder.RenameTable(
                name: "WmsDataSyncTasks",
                newName: "AppDataSyncTasks");

            migrationBuilder.RenameTable(
                name: "WmsDataSyncFieldMappings",
                newName: "AppDataSyncFieldMappings");

            migrationBuilder.RenameIndex(
                name: "IX_WmsProductionInbounds_WarehouseId",
                table: "AppProductionInbounds",
                newName: "IX_AppProductionInbounds_WarehouseId");

            migrationBuilder.RenameIndex(
                name: "IX_WmsProductionInbounds_SourceOrderNo",
                table: "AppProductionInbounds",
                newName: "IX_AppProductionInbounds_SourceOrderNo");

            migrationBuilder.RenameIndex(
                name: "IX_WmsProductionInbounds_OrderNo",
                table: "AppProductionInbounds",
                newName: "IX_AppProductionInbounds_OrderNo");

            migrationBuilder.RenameIndex(
                name: "IX_WmsProductionInbounds_DepartmentId",
                table: "AppProductionInbounds",
                newName: "IX_AppProductionInbounds_DepartmentId");

            migrationBuilder.RenameIndex(
                name: "IX_WmsProductionInboundDetails_SN",
                table: "AppProductionInboundDetails",
                newName: "IX_AppProductionInboundDetails_SN");

            migrationBuilder.RenameIndex(
                name: "IX_WmsProductionInboundDetails_ReelId",
                table: "AppProductionInboundDetails",
                newName: "IX_AppProductionInboundDetails_ReelId");

            migrationBuilder.RenameIndex(
                name: "IX_WmsProductionInboundDetails_ProductionInboundId",
                table: "AppProductionInboundDetails",
                newName: "IX_AppProductionInboundDetails_ProductionInboundId");

            migrationBuilder.RenameIndex(
                name: "IX_WmsProductionInboundDetails_BatchNo",
                table: "AppProductionInboundDetails",
                newName: "IX_AppProductionInboundDetails_BatchNo");

            migrationBuilder.RenameIndex(
                name: "IX_WmsDataSyncTasks_TaskCode",
                table: "AppDataSyncTasks",
                newName: "IX_AppDataSyncTasks_TaskCode");

            migrationBuilder.RenameIndex(
                name: "IX_WmsDataSyncFieldMappings_TaskId",
                table: "AppDataSyncFieldMappings",
                newName: "IX_AppDataSyncFieldMappings_TaskId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AppProductionInbounds",
                table: "AppProductionInbounds",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AppProductionInboundDetails",
                table: "AppProductionInboundDetails",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AppDataSyncTasks",
                table: "AppDataSyncTasks",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AppDataSyncFieldMappings",
                table: "AppDataSyncFieldMappings",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AppDataSyncFieldMappings_AppDataSyncTasks_TaskId",
                table: "AppDataSyncFieldMappings",
                column: "TaskId",
                principalTable: "AppDataSyncTasks",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AppProductionInboundDetails_AppProductionInbounds_Productio~",
                table: "AppProductionInboundDetails",
                column: "ProductionInboundId",
                principalTable: "AppProductionInbounds",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AppDataSyncFieldMappings_AppDataSyncTasks_TaskId",
                table: "AppDataSyncFieldMappings");

            migrationBuilder.DropForeignKey(
                name: "FK_AppProductionInboundDetails_AppProductionInbounds_Productio~",
                table: "AppProductionInboundDetails");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AppProductionInbounds",
                table: "AppProductionInbounds");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AppProductionInboundDetails",
                table: "AppProductionInboundDetails");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AppDataSyncTasks",
                table: "AppDataSyncTasks");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AppDataSyncFieldMappings",
                table: "AppDataSyncFieldMappings");

            migrationBuilder.RenameTable(
                name: "AppProductionInbounds",
                newName: "WmsProductionInbounds");

            migrationBuilder.RenameTable(
                name: "AppProductionInboundDetails",
                newName: "WmsProductionInboundDetails");

            migrationBuilder.RenameTable(
                name: "AppDataSyncTasks",
                newName: "WmsDataSyncTasks");

            migrationBuilder.RenameTable(
                name: "AppDataSyncFieldMappings",
                newName: "WmsDataSyncFieldMappings");

            migrationBuilder.RenameIndex(
                name: "IX_AppProductionInbounds_WarehouseId",
                table: "WmsProductionInbounds",
                newName: "IX_WmsProductionInbounds_WarehouseId");

            migrationBuilder.RenameIndex(
                name: "IX_AppProductionInbounds_SourceOrderNo",
                table: "WmsProductionInbounds",
                newName: "IX_WmsProductionInbounds_SourceOrderNo");

            migrationBuilder.RenameIndex(
                name: "IX_AppProductionInbounds_OrderNo",
                table: "WmsProductionInbounds",
                newName: "IX_WmsProductionInbounds_OrderNo");

            migrationBuilder.RenameIndex(
                name: "IX_AppProductionInbounds_DepartmentId",
                table: "WmsProductionInbounds",
                newName: "IX_WmsProductionInbounds_DepartmentId");

            migrationBuilder.RenameIndex(
                name: "IX_AppProductionInboundDetails_SN",
                table: "WmsProductionInboundDetails",
                newName: "IX_WmsProductionInboundDetails_SN");

            migrationBuilder.RenameIndex(
                name: "IX_AppProductionInboundDetails_ReelId",
                table: "WmsProductionInboundDetails",
                newName: "IX_WmsProductionInboundDetails_ReelId");

            migrationBuilder.RenameIndex(
                name: "IX_AppProductionInboundDetails_ProductionInboundId",
                table: "WmsProductionInboundDetails",
                newName: "IX_WmsProductionInboundDetails_ProductionInboundId");

            migrationBuilder.RenameIndex(
                name: "IX_AppProductionInboundDetails_BatchNo",
                table: "WmsProductionInboundDetails",
                newName: "IX_WmsProductionInboundDetails_BatchNo");

            migrationBuilder.RenameIndex(
                name: "IX_AppDataSyncTasks_TaskCode",
                table: "WmsDataSyncTasks",
                newName: "IX_WmsDataSyncTasks_TaskCode");

            migrationBuilder.RenameIndex(
                name: "IX_AppDataSyncFieldMappings_TaskId",
                table: "WmsDataSyncFieldMappings",
                newName: "IX_WmsDataSyncFieldMappings_TaskId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_WmsProductionInbounds",
                table: "WmsProductionInbounds",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_WmsProductionInboundDetails",
                table: "WmsProductionInboundDetails",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_WmsDataSyncTasks",
                table: "WmsDataSyncTasks",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_WmsDataSyncFieldMappings",
                table: "WmsDataSyncFieldMappings",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_WmsDataSyncFieldMappings_WmsDataSyncTasks_TaskId",
                table: "WmsDataSyncFieldMappings",
                column: "TaskId",
                principalTable: "WmsDataSyncTasks",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_WmsProductionInboundDetails_WmsProductionInbounds_Productio~",
                table: "WmsProductionInboundDetails",
                column: "ProductionInboundId",
                principalTable: "WmsProductionInbounds",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
