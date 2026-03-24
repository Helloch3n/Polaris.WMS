using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Polaris.WMS.Migrations
{
    public partial class _2616002 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
ALTER TABLE "AppLocations"
  ALTER COLUMN "Type" TYPE integer USING
    CASE
      WHEN "Type"::text = 'Dock' THEN 10
      WHEN "Type"::text = 'Storage' THEN 20
      WHEN "Type"::text = 'LineSide' THEN 30
      WHEN "Type"::text = 'QC' THEN 40
      WHEN "Type"::text = 'Outbound' THEN 50
      ELSE "Type"::integer
    END;
""");

            migrationBuilder.Sql("""
ALTER TABLE "AppLocations"
  ALTER COLUMN "Status" TYPE integer USING
    CASE
      WHEN "Status"::text = 'Idle' THEN 0
      WHEN "Status"::text = 'Partial' THEN 10
      WHEN "Status"::text = 'Full' THEN 20
      WHEN "Status"::text = 'Locked' THEN 30
      ELSE "Status"::integer
    END;
""");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
ALTER TABLE "AppLocations"
  ALTER COLUMN "Type" TYPE text USING
    CASE
      WHEN "Type" = 10 THEN 'Dock'
      WHEN "Type" = 20 THEN 'Storage'
      WHEN "Type" = 30 THEN 'LineSide'
      WHEN "Type" = 40 THEN 'QC'
      WHEN "Type" = 50 THEN 'Outbound'
      ELSE "Type"::text
    END;
""");

            migrationBuilder.Sql("""
ALTER TABLE "AppLocations"
  ALTER COLUMN "Status" TYPE text USING
    CASE
      WHEN "Status" = 0 THEN 'Idle'
      WHEN "Status" = 10 THEN 'Partial'
      WHEN "Status" = 20 THEN 'Full'
      WHEN "Status" = 30 THEN 'Locked'
      ELSE "Status"::text
    END;
""");
        }
    }
}

