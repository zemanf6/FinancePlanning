using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FinancePlanning.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UserPropChange : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "BirthDate",
                table: "AspNetUsers",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "DefaultRiskProfile",
                table: "AspNetUsers",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DefaultTargetRetirementAge",
                table: "AspNetUsers",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "PreferredCurrency",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BirthDate",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "DefaultRiskProfile",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "DefaultTargetRetirementAge",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "PreferredCurrency",
                table: "AspNetUsers");
        }
    }
}
