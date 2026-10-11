using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PlayerApp.Models.Migrations
{
    /// <inheritdoc />
    public partial class ACDRAndHealthUpdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BaseArmorClass",
                table: "Characters");

            migrationBuilder.RenameColumn(
                name: "MaxHealth",
                table: "Characters",
                newName: "BaseHealth");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "BaseHealth",
                table: "Characters",
                newName: "MaxHealth");

            migrationBuilder.AddColumn<int>(
                name: "BaseArmorClass",
                table: "Characters",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }
    }
}
