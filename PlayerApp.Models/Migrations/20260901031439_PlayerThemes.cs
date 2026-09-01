using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PlayerApp.Models.Migrations
{
    /// <inheritdoc />
    public partial class PlayerThemes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Theme",
                table: "Players",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Theme",
                table: "Players");
        }
    }
}
