using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PlayerApp.Models.Migrations
{
    /// <inheritdoc />
    public partial class Talents : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Perk",
                table: "ScifiTalents",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "PerkAmount",
                table: "ScifiTalents",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "ScifiCharacterTalents",
                columns: table => new
                {
                    ScifiCharacterId = table.Column<int>(type: "integer", nullable: false),
                    ScifiTalentId = table.Column<int>(type: "integer", nullable: false),
                    SourceType = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ScifiCharacterTalents", x => new { x.ScifiCharacterId, x.ScifiTalentId });
                    table.ForeignKey(
                        name: "FK_ScifiCharacterTalents_ScifiCharacters_ScifiCharacterId",
                        column: x => x.ScifiCharacterId,
                        principalTable: "ScifiCharacters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ScifiCharacterTalents_ScifiTalents_ScifiTalentId",
                        column: x => x.ScifiTalentId,
                        principalTable: "ScifiTalents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ScifiCharacterTalents_ScifiTalentId",
                table: "ScifiCharacterTalents",
                column: "ScifiTalentId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ScifiCharacterTalents");

            migrationBuilder.DropColumn(
                name: "Perk",
                table: "ScifiTalents");

            migrationBuilder.DropColumn(
                name: "PerkAmount",
                table: "ScifiTalents");
        }
    }
}
