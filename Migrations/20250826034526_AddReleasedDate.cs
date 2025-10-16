using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ServiceCatalogBGC_V2.Migrations
{
    /// <inheritdoc />
    public partial class AddReleasedDate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "ReleasedDate",
                table: "Catalogs",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ReleasedDate",
                table: "Catalogs");
        }
    }
}
