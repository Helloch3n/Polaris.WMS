using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Polaris.WMS.Migrations
{
    /// <inheritdoc />
    public partial class addaccountalias : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SourceDetailId",
                table: "AppPurchaseReceiptDetails");

            migrationBuilder.CreateTable(
                name: "AppAccountAliases",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Alias = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false, comment: "账户别名"),
                    Description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false, comment: "账户别名说明"),
                    EffectiveDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, comment: "生效日期"),
                    ExpireDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true, comment: "失效日期"),
                    IsUnitPriceRequired = table.Column<bool>(type: "boolean", nullable: false, comment: "是否填写单价"),
                    IsProjectRequired = table.Column<bool>(type: "boolean", nullable: false, comment: "是否填写项目"),
                    IsDepartmentRequired = table.Column<bool>(type: "boolean", nullable: false, comment: "是否填写部门"),
                    IsProductionNoRequired = table.Column<bool>(type: "boolean", nullable: false, comment: "是否填写生产号"),
                    IsWorkOrderOperationRequired = table.Column<bool>(type: "boolean", nullable: false, comment: "是否填写工单工序"),
                    ProductionCostType = table.Column<int>(type: "integer", nullable: false, comment: "生产成本类型"),
                    IsSupplierRequired = table.Column<bool>(type: "boolean", nullable: false, comment: "是否填写供应商"),
                    IsCustomerRequired = table.Column<bool>(type: "boolean", nullable: false, comment: "是否填写客户"),
                    IsWorkOrderAttributeRequired = table.Column<bool>(type: "boolean", nullable: false, comment: "是否填写工单属性"),
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
                    table.PrimaryKey("PK_AppAccountAliases", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AppCostCenters",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Code = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false, comment: "成本中心编码"),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false, comment: "成本中心名称"),
                    DepartmentCode = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false, comment: "部门编码"),
                    DepartmentName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false, comment: "部门名称"),
                    CompanyCode = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false, comment: "公司编码"),
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
                    table.PrimaryKey("PK_AppCostCenters", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AppAccountAliases_Alias",
                table: "AppAccountAliases",
                column: "Alias",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AppCostCenters_Code",
                table: "AppCostCenters",
                column: "Code",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AppAccountAliases");

            migrationBuilder.DropTable(
                name: "AppCostCenters");

            migrationBuilder.AddColumn<Guid>(
                name: "SourceDetailId",
                table: "AppPurchaseReceiptDetails",
                type: "uuid",
                nullable: true);
        }
    }
}
