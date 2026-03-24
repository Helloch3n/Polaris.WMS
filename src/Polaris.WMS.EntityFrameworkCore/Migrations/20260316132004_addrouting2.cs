using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Polaris.WMS.Migrations
{
    /// <inheritdoc />
    public partial class addrouting2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_RoutingStrategy",
                table: "RoutingStrategy");

            migrationBuilder.RenameTable(
                name: "RoutingStrategy",
                newName: "AppRoutingStrategies");

            migrationBuilder.AlterColumn<int>(
                name: "TaskType",
                table: "AppRoutingStrategies",
                type: "integer",
                nullable: false,
                comment: "任务类型",
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<Guid>(
                name: "TargetZoneId",
                table: "AppRoutingStrategies",
                type: "uuid",
                nullable: false,
                comment: "目标分区/库区 Id",
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AlterColumn<Guid>(
                name: "SourceZoneId",
                table: "AppRoutingStrategies",
                type: "uuid",
                nullable: true,
                comment: "来源分区/库区 Id（可选）",
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "RuleName",
                table: "AppRoutingStrategies",
                type: "character varying(200)",
                maxLength: 200,
                nullable: false,
                comment: "路由规则名称",
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<Guid>(
                name: "ProductId",
                table: "AppRoutingStrategies",
                type: "uuid",
                nullable: true,
                comment: "物料 Id（可选）",
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "ProductCategoryId",
                table: "AppRoutingStrategies",
                type: "uuid",
                nullable: true,
                comment: "物料类别 Id（可选）",
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "Priority",
                table: "AppRoutingStrategies",
                type: "integer",
                nullable: false,
                comment: "规则优先级，数字越小越优先",
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<bool>(
                name: "IsActive",
                table: "AppRoutingStrategies",
                type: "boolean",
                nullable: false,
                defaultValue: true,
                comment: "是否启用",
                oldClrType: typeof(bool),
                oldType: "boolean");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AppRoutingStrategies",
                table: "AppRoutingStrategies",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_AppRoutingStrategies_RuleName",
                table: "AppRoutingStrategies",
                column: "RuleName");

            migrationBuilder.CreateIndex(
                name: "IX_AppRoutingStrategies_TaskType_IsActive_Priority",
                table: "AppRoutingStrategies",
                columns: new[] { "TaskType", "IsActive", "Priority" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_AppRoutingStrategies",
                table: "AppRoutingStrategies");

            migrationBuilder.DropIndex(
                name: "IX_AppRoutingStrategies_RuleName",
                table: "AppRoutingStrategies");

            migrationBuilder.DropIndex(
                name: "IX_AppRoutingStrategies_TaskType_IsActive_Priority",
                table: "AppRoutingStrategies");

            migrationBuilder.RenameTable(
                name: "AppRoutingStrategies",
                newName: "RoutingStrategy");

            migrationBuilder.AlterColumn<int>(
                name: "TaskType",
                table: "RoutingStrategy",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer",
                oldComment: "任务类型");

            migrationBuilder.AlterColumn<Guid>(
                name: "TargetZoneId",
                table: "RoutingStrategy",
                type: "uuid",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldComment: "目标分区/库区 Id");

            migrationBuilder.AlterColumn<Guid>(
                name: "SourceZoneId",
                table: "RoutingStrategy",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true,
                oldComment: "来源分区/库区 Id（可选）");

            migrationBuilder.AlterColumn<string>(
                name: "RuleName",
                table: "RoutingStrategy",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(200)",
                oldMaxLength: 200,
                oldComment: "路由规则名称");

            migrationBuilder.AlterColumn<Guid>(
                name: "ProductId",
                table: "RoutingStrategy",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true,
                oldComment: "物料 Id（可选）");

            migrationBuilder.AlterColumn<Guid>(
                name: "ProductCategoryId",
                table: "RoutingStrategy",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true,
                oldComment: "物料类别 Id（可选）");

            migrationBuilder.AlterColumn<int>(
                name: "Priority",
                table: "RoutingStrategy",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer",
                oldComment: "规则优先级，数字越小越优先");

            migrationBuilder.AlterColumn<bool>(
                name: "IsActive",
                table: "RoutingStrategy",
                type: "boolean",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "boolean",
                oldDefaultValue: true,
                oldComment: "是否启用");

            migrationBuilder.AddPrimaryKey(
                name: "PK_RoutingStrategy",
                table: "RoutingStrategy",
                column: "Id");
        }
    }
}
