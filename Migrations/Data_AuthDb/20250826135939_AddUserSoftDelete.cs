using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ServiceCatalogBGC_V2.Migrations.Data_AuthDb
{
    /// <inheritdoc />
    public partial class AddUserSoftDelete : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "DisabledAt",
                table: "AspNetUsers",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DisabledBy",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDisabled",
                table: "AspNetUsers",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DisabledAt",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "DisabledBy",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "IsDisabled",
                table: "AspNetUsers");
        }
    }
}
