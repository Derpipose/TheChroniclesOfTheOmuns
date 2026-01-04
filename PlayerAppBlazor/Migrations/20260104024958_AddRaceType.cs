using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PlayerAppBlazor.Migrations
{
    /// <inheritdoc />
    public partial class AddRaceType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "RaceType",
                table: "CharacterRaces",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RaceType",
                table: "CharacterRaces");
        }
    }
}
