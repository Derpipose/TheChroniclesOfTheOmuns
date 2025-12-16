using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PlayerApp.Migrations; 
/// <inheritdoc />
public partial class InitialCreate : Migration {
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder) {
        migrationBuilder.CreateTable(
            name: "CharacterClass",
            columns: table => new {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                Description = table.Column<string>(type: "nvarchar(max)", nullable: false)
            },
            constraints: table => {
                table.PrimaryKey("PK_CharacterClass", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "CharacterRace",
            columns: table => new {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                Description = table.Column<string>(type: "nvarchar(max)", nullable: false)
            },
            constraints: table => {
                table.PrimaryKey("PK_CharacterRace", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "Character",
            columns: table => new {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                Level = table.Column<int>(type: "int", nullable: false),
                CharacterRaceId = table.Column<int>(type: "int", nullable: true),
                CharacterClassId = table.Column<int>(type: "int", nullable: true)
            },
            constraints: table => {
                table.PrimaryKey("PK_Character", x => x.Id);
                table.ForeignKey(
                    name: "FK_Character_CharacterClass_CharacterClassId",
                    column: x => x.CharacterClassId,
                    principalTable: "CharacterClass",
                    principalColumn: "Id");
                table.ForeignKey(
                    name: "FK_Character_CharacterRace_CharacterRaceId",
                    column: x => x.CharacterRaceId,
                    principalTable: "CharacterRace",
                    principalColumn: "Id");
            });

        migrationBuilder.CreateTable(
            name: "CharacterStats",
            columns: table => new {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                CharacterId = table.Column<int>(type: "int", nullable: false),
                Strength = table.Column<int>(type: "int", nullable: false),
                Constitution = table.Column<int>(type: "int", nullable: false),
                Dexterity = table.Column<int>(type: "int", nullable: false),
                Wisdom = table.Column<int>(type: "int", nullable: false),
                Charisma = table.Column<int>(type: "int", nullable: false),
                Intelligence = table.Column<int>(type: "int", nullable: false)
            },
            constraints: table => {
                table.PrimaryKey("PK_CharacterStats", x => x.Id);
                table.ForeignKey(
                    name: "FK_CharacterStats_Character_CharacterId",
                    column: x => x.CharacterId,
                    principalTable: "Character",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateIndex(
            name: "IX_Character_CharacterClassId",
            table: "Character",
            column: "CharacterClassId");

        migrationBuilder.CreateIndex(
            name: "IX_Character_CharacterRaceId",
            table: "Character",
            column: "CharacterRaceId");

        migrationBuilder.CreateIndex(
            name: "IX_CharacterStats_CharacterId",
            table: "CharacterStats",
            column: "CharacterId",
            unique: true);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder) {
        migrationBuilder.DropTable(
            name: "CharacterStats");

        migrationBuilder.DropTable(
            name: "Character");

        migrationBuilder.DropTable(
            name: "CharacterClass");

        migrationBuilder.DropTable(
            name: "CharacterRace");
    }
}
