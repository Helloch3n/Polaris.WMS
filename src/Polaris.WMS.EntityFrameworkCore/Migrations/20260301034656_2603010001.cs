using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Polaris.WMS.Migrations
{
    /// <inheritdoc />
    public partial class _2603010001 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "TaskName",
                table: "WmsDataSyncTasks",
                type: "character varying(128)",
                maxLength: 128,
                nullable: false,
                comment: "浠诲姟鍚嶇О",
                oldClrType: typeof(string),
                oldType: "character varying(128)",
                oldMaxLength: 128,
                oldComment: "任务名称");

            migrationBuilder.AlterColumn<string>(
                name: "TaskCode",
                table: "WmsDataSyncTasks",
                type: "character varying(64)",
                maxLength: 64,
                nullable: false,
                comment: "浠诲姟缂栫爜",
                oldClrType: typeof(string),
                oldType: "character varying(64)",
                oldMaxLength: 64,
                oldComment: "任务编码");

            migrationBuilder.AddColumn<string>(
                name: "SyncUrl",
                table: "WmsDataSyncTasks",
                type: "character varying(256)",
                maxLength: 256,
                nullable: false,
                defaultValue: "",
                comment: "鍚屾鍦板潃");

            migrationBuilder.AddColumn<string>(
                name: "TargetTable",
                table: "WmsDataSyncTasks",
                type: "character varying(128)",
                maxLength: 128,
                nullable: false,
                defaultValue: "",
                comment: "鐩爣琛ㄥ悕");

            migrationBuilder.AddColumn<int>(
                name: "ReelType",
                table: "AppReels",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<string>(
                name: "ToLocation",
                table: "AppPickTasks",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true,
                comment: "鐩爣搴撲綅",
                oldClrType: typeof(string),
                oldType: "character varying(50)",
                oldMaxLength: 50,
                oldNullable: true,
                oldComment: "目标库位");

            migrationBuilder.AlterColumn<string>(
                name: "ReelNo",
                table: "AppPickTasks",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                comment: "鐩樺叿缂栧彿",
                oldClrType: typeof(string),
                oldType: "character varying(50)",
                oldMaxLength: 50,
                oldComment: "盘具编号");

            migrationBuilder.AlterColumn<string>(
                name: "FromLocation",
                table: "AppPickTasks",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true,
                comment: "鏉ユ簮搴撲綅",
                oldClrType: typeof(string),
                oldType: "character varying(50)",
                oldMaxLength: 50,
                oldNullable: true,
                oldComment: "来源库位");

            migrationBuilder.CreateTable(
                name: "WmsDataSyncFieldMappings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TaskId = table.Column<Guid>(type: "uuid", nullable: false, comment: "鎵€灞炰换鍔?ID"),
                    SourceField = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false, comment: "鏉ユ簮瀛楁"),
                    TargetField = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false, comment: "鐩爣瀛楁")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WmsDataSyncFieldMappings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WmsDataSyncFieldMappings_WmsDataSyncTasks_TaskId",
                        column: x => x.TaskId,
                        principalTable: "WmsDataSyncTasks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_WmsDataSyncFieldMappings_TaskId",
                table: "WmsDataSyncFieldMappings",
                column: "TaskId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "WmsDataSyncFieldMappings");

            migrationBuilder.DropColumn(
                name: "SyncUrl",
                table: "WmsDataSyncTasks");

            migrationBuilder.DropColumn(
                name: "TargetTable",
                table: "WmsDataSyncTasks");

            migrationBuilder.DropColumn(
                name: "ReelType",
                table: "AppReels");

            migrationBuilder.AlterColumn<string>(
                name: "TaskName",
                table: "WmsDataSyncTasks",
                type: "character varying(128)",
                maxLength: 128,
                nullable: false,
                comment: "任务名称",
                oldClrType: typeof(string),
                oldType: "character varying(128)",
                oldMaxLength: 128,
                oldComment: "浠诲姟鍚嶇О");

            migrationBuilder.AlterColumn<string>(
                name: "TaskCode",
                table: "WmsDataSyncTasks",
                type: "character varying(64)",
                maxLength: 64,
                nullable: false,
                comment: "任务编码",
                oldClrType: typeof(string),
                oldType: "character varying(64)",
                oldMaxLength: 64,
                oldComment: "浠诲姟缂栫爜");

            migrationBuilder.AlterColumn<string>(
                name: "ToLocation",
                table: "AppPickTasks",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true,
                comment: "目标库位",
                oldClrType: typeof(string),
                oldType: "character varying(50)",
                oldMaxLength: 50,
                oldNullable: true,
                oldComment: "鐩爣搴撲綅");

            migrationBuilder.AlterColumn<string>(
                name: "ReelNo",
                table: "AppPickTasks",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                comment: "盘具编号",
                oldClrType: typeof(string),
                oldType: "character varying(50)",
                oldMaxLength: 50,
                oldComment: "鐩樺叿缂栧彿");

            migrationBuilder.AlterColumn<string>(
                name: "FromLocation",
                table: "AppPickTasks",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true,
                comment: "来源库位",
                oldClrType: typeof(string),
                oldType: "character varying(50)",
                oldMaxLength: 50,
                oldNullable: true,
                oldComment: "鏉ユ簮搴撲綅");
        }
    }
}
