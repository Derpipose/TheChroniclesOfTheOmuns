using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace PlayerApp.Models.Migrations
{
    /// <inheritdoc />
    public partial class AddSkills : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ScifiSkills",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    StatGovernor = table.Column<string>(type: "text", nullable: false),
                    SkillType = table.Column<string>(type: "text", nullable: false),
                    SkillKind = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ScifiSkills", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ScifiCharacterSkills",
                columns: table => new
                {
                    ScifiCharacterId = table.Column<int>(type: "integer", nullable: false),
                    ScifiSkillId = table.Column<int>(type: "integer", nullable: false),
                    SourceType = table.Column<string>(type: "text", nullable: false),
                    SkillAmount = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ScifiCharacterSkills", x => new { x.ScifiCharacterId, x.ScifiSkillId, x.SourceType });
                    table.ForeignKey(
                        name: "FK_ScifiCharacterSkills_ScifiCharacters_ScifiCharacterId",
                        column: x => x.ScifiCharacterId,
                        principalTable: "ScifiCharacters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ScifiCharacterSkills_ScifiSkills_ScifiSkillId",
                        column: x => x.ScifiSkillId,
                        principalTable: "ScifiSkills",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ScifiCharacterSkills_ScifiSkillId",
                table: "ScifiCharacterSkills",
                column: "ScifiSkillId");

            migrationBuilder.CreateIndex(
                name: "IX_ScifiSkills_Name_SkillKind",
                table: "ScifiSkills",
                columns: new[] { "Name", "SkillKind" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ScifiCharacterSkills");

            migrationBuilder.DropTable(
                name: "ScifiSkills");
        }
    }
}
