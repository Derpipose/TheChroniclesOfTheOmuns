using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PlayerApp.Models.Migrations
{
    /// <inheritdoc />
    public partial class LegaciedAndBaseCharacters : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CharacterStats",
                columns: table => new
                {
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
                constraints: table =>
                {
                    table.PrimaryKey("PK_CharacterStats", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "FantasyCharacterClasses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FantasyClassType = table.Column<int>(type: "int", nullable: false),
                    IsVeteranLocked = table.Column<bool>(type: "bit", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    HitDice = table.Column<int>(type: "int", nullable: false),
                    ManaDice = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FantasyCharacterClasses", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "FantasyCharacterRaces",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RaceType = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FantasyCharacterRaces", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Modifiers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Type = table.Column<int>(type: "int", nullable: false),
                    Value = table.Column<int>(type: "int", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Modifiers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "FantasyCharacters",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Level = table.Column<int>(type: "int", nullable: false),
                    StatsId = table.Column<int>(type: "int", nullable: false),
                    Health = table.Column<int>(type: "int", nullable: false),
                    Mana = table.Column<int>(type: "int", nullable: false),
                    FantasyCharacterRaceId = table.Column<int>(type: "int", nullable: true),
                    FantasyCharacterClassId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FantasyCharacters", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FantasyCharacters_CharacterStats_StatsId",
                        column: x => x.StatsId,
                        principalTable: "CharacterStats",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FantasyCharacters_FantasyCharacterClasses_FantasyCharacterClassId",
                        column: x => x.FantasyCharacterClassId,
                        principalTable: "FantasyCharacterClasses",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_FantasyCharacters_FantasyCharacterRaces_FantasyCharacterRaceId",
                        column: x => x.FantasyCharacterRaceId,
                        principalTable: "FantasyCharacterRaces",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "FantasyRaceBonus",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RaceId = table.Column<int>(type: "int", nullable: false),
                    BonusValue = table.Column<int>(type: "int", nullable: false),
                    StatId = table.Column<int>(type: "int", nullable: true),
                    IsSelectable = table.Column<bool>(type: "bit", nullable: false),
                    FantasyCharacterRaceId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FantasyRaceBonus", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FantasyRaceBonus_FantasyCharacterRaces_FantasyCharacterRaceId",
                        column: x => x.FantasyCharacterRaceId,
                        principalTable: "FantasyCharacterRaces",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "RacialModifiers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RaceId = table.Column<int>(type: "int", nullable: false),
                    ModifierId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RacialModifiers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RacialModifiers_FantasyCharacterRaces_RaceId",
                        column: x => x.RaceId,
                        principalTable: "FantasyCharacterRaces",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RacialModifiers_Modifiers_ModifierId",
                        column: x => x.ModifierId,
                        principalTable: "Modifiers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CharacterStatBonus",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CharacterId = table.Column<int>(type: "int", nullable: false),
                    BonusValue = table.Column<int>(type: "int", nullable: false),
                    BonusSource = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    StatId = table.Column<int>(type: "int", nullable: true),
                    IsSelectable = table.Column<bool>(type: "bit", nullable: false),
                    FantasyCharacterId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CharacterStatBonus", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CharacterStatBonus_FantasyCharacters_FantasyCharacterId",
                        column: x => x.FantasyCharacterId,
                        principalTable: "FantasyCharacters",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_CharacterStatBonus_FantasyCharacterId",
                table: "CharacterStatBonus",
                column: "FantasyCharacterId");

            migrationBuilder.CreateIndex(
                name: "IX_FantasyCharacters_FantasyCharacterClassId",
                table: "FantasyCharacters",
                column: "FantasyCharacterClassId");

            migrationBuilder.CreateIndex(
                name: "IX_FantasyCharacters_FantasyCharacterRaceId",
                table: "FantasyCharacters",
                column: "FantasyCharacterRaceId");

            migrationBuilder.CreateIndex(
                name: "IX_FantasyCharacters_StatsId",
                table: "FantasyCharacters",
                column: "StatsId");

            migrationBuilder.CreateIndex(
                name: "IX_FantasyRaceBonus_FantasyCharacterRaceId",
                table: "FantasyRaceBonus",
                column: "FantasyCharacterRaceId");

            migrationBuilder.CreateIndex(
                name: "IX_RacialModifiers_ModifierId",
                table: "RacialModifiers",
                column: "ModifierId");

            migrationBuilder.CreateIndex(
                name: "IX_RacialModifiers_RaceId",
                table: "RacialModifiers",
                column: "RaceId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CharacterStatBonus");

            migrationBuilder.DropTable(
                name: "FantasyRaceBonus");

            migrationBuilder.DropTable(
                name: "RacialModifiers");

            migrationBuilder.DropTable(
                name: "FantasyCharacters");

            migrationBuilder.DropTable(
                name: "Modifiers");

            migrationBuilder.DropTable(
                name: "CharacterStats");

            migrationBuilder.DropTable(
                name: "FantasyCharacterClasses");

            migrationBuilder.DropTable(
                name: "FantasyCharacterRaces");
        }
    }
}
