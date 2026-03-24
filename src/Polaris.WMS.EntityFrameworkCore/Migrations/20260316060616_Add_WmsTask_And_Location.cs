using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Polaris.WMS.Migrations
{
    /// <inheritdoc />
    public partial class Add_WmsTask_And_Location : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AppMoveTasks",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TaskNo = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false, comment: "任务编号"),
                    TaskType = table.Column<int>(type: "integer", nullable: false, comment: "任务类型（枚举）"),
                    Status = table.Column<int>(type: "integer", nullable: false, comment: "任务状态（枚举）"),
                    ContainerId = table.Column<Guid>(type: "uuid", nullable: false, comment: "绑定的物理载具Id"),
                    SourceLocationId = table.Column<Guid>(type: "uuid", nullable: true, comment: "源库位Id"),
                    TargetLocationId = table.Column<Guid>(type: "uuid", nullable: false, comment: "计划目标库位Id"),
                    ActualLocationId = table.Column<Guid>(type: "uuid", nullable: true, comment: "实际落位库位Id（完成时由PDA扫码记录）"),
                    ExtraProperties = table.Column<string>(type: "text", nullable: false),
                    ConcurrencyStamp = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    CreationTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    CreatorId = table.Column<Guid>(type: "uuid", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    LastModifierId = table.Column<Guid>(type: "uuid", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    DeleterId = table.Column<Guid>(type: "uuid", nullable: true),
                    DeletionTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppMoveTasks", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AppMoveTasks_ContainerId",
                table: "AppMoveTasks",
                column: "ContainerId");

            migrationBuilder.CreateIndex(
                name: "IX_AppMoveTasks_TaskNo",
                table: "AppMoveTasks",
                column: "TaskNo");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AppMoveTasks");
        }
    }
}
