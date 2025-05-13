using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FinancePlanning.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class NoteAdded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Note",
                table: "SavedSimpleInterests",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Note",
                table: "SavedSimpleInterests");
        }
    }
}
