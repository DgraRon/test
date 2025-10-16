using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ServiceCatalogBGC_V2.Migrations
{
    /// <inheritdoc />
    public partial class AddAuditFieldsToCatalog : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "Statuses",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedDate",
                table: "Statuses",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ModifiedBy",
                table: "Statuses",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ModifiedDate",
                table: "Statuses",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "ServiceType",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedDate",
                table: "ServiceType",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ModifiedBy",
                table: "ServiceType",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ModifiedDate",
                table: "ServiceType",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "Priorities",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedDate",
                table: "Priorities",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ModifiedBy",
                table: "Priorities",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ModifiedDate",
                table: "Priorities",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "DeploymentTypes",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedDate",
                table: "DeploymentTypes",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ModifiedBy",
                table: "DeploymentTypes",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ModifiedDate",
                table: "DeploymentTypes",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "Categories",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedDate",
                table: "Categories",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ModifiedBy",
                table: "Categories",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ModifiedDate",
                table: "Categories",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "Catalogs",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedDate",
                table: "Catalogs",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ModifiedBy",
                table: "Catalogs",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ModifiedDate",
                table: "Catalogs",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "Statuses");

            migrationBuilder.DropColumn(
                name: "CreatedDate",
                table: "Statuses");

            migrationBuilder.DropColumn(
                name: "ModifiedBy",
                table: "Statuses");

            migrationBuilder.DropColumn(
                name: "ModifiedDate",
                table: "Statuses");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "ServiceType");

            migrationBuilder.DropColumn(
                name: "CreatedDate",
                table: "ServiceType");

            migrationBuilder.DropColumn(
                name: "ModifiedBy",
                table: "ServiceType");

            migrationBuilder.DropColumn(
                name: "ModifiedDate",
                table: "ServiceType");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "Priorities");

            migrationBuilder.DropColumn(
                name: "CreatedDate",
                table: "Priorities");

            migrationBuilder.DropColumn(
                name: "ModifiedBy",
                table: "Priorities");

            migrationBuilder.DropColumn(
                name: "ModifiedDate",
                table: "Priorities");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "DeploymentTypes");

            migrationBuilder.DropColumn(
                name: "CreatedDate",
                table: "DeploymentTypes");

            migrationBuilder.DropColumn(
                name: "ModifiedBy",
                table: "DeploymentTypes");

            migrationBuilder.DropColumn(
                name: "ModifiedDate",
                table: "DeploymentTypes");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "Categories");

            migrationBuilder.DropColumn(
                name: "CreatedDate",
                table: "Categories");

            migrationBuilder.DropColumn(
                name: "ModifiedBy",
                table: "Categories");

            migrationBuilder.DropColumn(
                name: "ModifiedDate",
                table: "Categories");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "Catalogs");

            migrationBuilder.DropColumn(
                name: "CreatedDate",
                table: "Catalogs");

            migrationBuilder.DropColumn(
                name: "ModifiedBy",
                table: "Catalogs");

            migrationBuilder.DropColumn(
                name: "ModifiedDate",
                table: "Catalogs");
        }
    }
}
