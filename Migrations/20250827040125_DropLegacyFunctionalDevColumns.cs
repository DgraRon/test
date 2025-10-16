using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ServiceCatalogBGC_V2.Migrations
{
    /// <inheritdoc />
    public partial class DropLegacyFunctionalDevColumns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // ลบคอลัมน์เก่าแบบมีเงื่อนไข (กัน error ถ้าคอลัมน์ถูกลบไปแล้ว)
            migrationBuilder.Sql(@"
IF EXISTS (SELECT 1 FROM sys.columns 
           WHERE [name] = N'Functional' AND object_id = OBJECT_ID(N'dbo.Catalogs'))
BEGIN
    ALTER TABLE dbo.Catalogs DROP COLUMN [Functional];
END

IF EXISTS (SELECT 1 FROM sys.columns 
           WHERE [name] = N'Dev' AND object_id = OBJECT_ID(N'dbo.Catalogs'))
BEGIN
    ALTER TABLE dbo.Catalogs DROP COLUMN [Dev];
END
");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // เพิ่มคอลัมน์กลับแบบมีเช็ค (กัน error ถ้าคอลัมน์มีอยู่แล้ว)
            migrationBuilder.Sql(@"
IF COL_LENGTH('dbo.Catalogs', 'Dev') IS NULL
BEGIN
    ALTER TABLE dbo.Catalogs ADD [Dev] nvarchar(max) NULL;
END

IF COL_LENGTH('dbo.Catalogs', 'Functional') IS NULL
BEGIN
    ALTER TABLE dbo.Catalogs ADD [Functional] nvarchar(max) NULL;
END
");
        }
    }
}
