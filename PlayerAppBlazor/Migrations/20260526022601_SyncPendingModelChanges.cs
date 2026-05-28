using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PlayerAppBlazor.Migrations
{
    /// <inheritdoc />
    public partial class SyncPendingModelChanges : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CharacterStatBonus_Characters_CharacterId",
                table: "CharacterStatBonus");

            migrationBuilder.DropForeignKey(
                name: "FK_CharacterStats_Characters_CharacterId",
                table: "CharacterStats");

            migrationBuilder.DropForeignKey(
                name: "FK_RacialModifiers_CharacterRaces_RaceId",
                table: "RacialModifiers");

            migrationBuilder.DropTable(
                name: "Characters");

            migrationBuilder.DropTable(
                name: "RaceStatBonus");

            migrationBuilder.DropTable(
                name: "CharacterClasses");

            migrationBuilder.DropTable(
                name: "CharacterRaces");

            migrationBuilder.DropIndex(
                name: "IX_CharacterStats_CharacterId",
                table: "CharacterStats");

            migrationBuilder.DropIndex(
                name: "IX_CharacterStatBonus_CharacterId",
                table: "CharacterStatBonus");

            migrationBuilder.AddColumn<int>(
                name: "FantasyCharacterId",
                table: "CharacterStatBonus",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "FantasyCharacterClasses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    FantasyClassType = table.Column<int>(type: "INTEGER", nullable: false),
                    IsVeteranLocked = table.Column<bool>(type: "INTEGER", nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: false),
                    HitDice = table.Column<int>(type: "INTEGER", nullable: false),
                    ManaDice = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FantasyCharacterClasses", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "FantasyCharacterRaces",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: false),
                    RaceType = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FantasyCharacterRaces", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "FantasyCharacters",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Level = table.Column<int>(type: "INTEGER", nullable: false),
                    StatsId = table.Column<int>(type: "INTEGER", nullable: false),
                    Health = table.Column<int>(type: "INTEGER", nullable: false),
                    Mana = table.Column<int>(type: "INTEGER", nullable: false),
                    FantasyCharacterRaceId = table.Column<int>(type: "INTEGER", nullable: true),
                    FantasyCharacterClassId = table.Column<int>(type: "INTEGER", nullable: true)
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
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    RaceId = table.Column<int>(type: "INTEGER", nullable: false),
                    BonusValue = table.Column<int>(type: "INTEGER", nullable: false),
                    StatId = table.Column<int>(type: "INTEGER", nullable: true),
                    IsSelectable = table.Column<bool>(type: "INTEGER", nullable: false),
                    FantasyCharacterRaceId = table.Column<int>(type: "INTEGER", nullable: true)
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

            migrationBuilder.AddForeignKey(
                name: "FK_CharacterStatBonus_FantasyCharacters_FantasyCharacterId",
                table: "CharacterStatBonus",
                column: "FantasyCharacterId",
                principalTable: "FantasyCharacters",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_RacialModifiers_FantasyCharacterRaces_RaceId",
                table: "RacialModifiers",
                column: "RaceId",
                principalTable: "FantasyCharacterRaces",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CharacterStatBonus_FantasyCharacters_FantasyCharacterId",
                table: "CharacterStatBonus");

            migrationBuilder.DropForeignKey(
                name: "FK_RacialModifiers_FantasyCharacterRaces_RaceId",
                table: "RacialModifiers");

            migrationBuilder.DropTable(
                name: "FantasyCharacters");

            migrationBuilder.DropTable(
                name: "FantasyRaceBonus");

            migrationBuilder.DropTable(
                name: "FantasyCharacterClasses");

            migrationBuilder.DropTable(
                name: "FantasyCharacterRaces");

            migrationBuilder.DropIndex(
                name: "IX_CharacterStatBonus_FantasyCharacterId",
                table: "CharacterStatBonus");

            migrationBuilder.DropColumn(
                name: "FantasyCharacterId",
                table: "CharacterStatBonus");

            migrationBuilder.CreateTable(
                name: "CharacterClasses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ClassType = table.Column<int>(type: "INTEGER", nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: false),
                    HitDice = table.Column<int>(type: "INTEGER", nullable: false),
                    IsVeteranLocked = table.Column<bool>(type: "INTEGER", nullable: false),
                    ManaDice = table.Column<int>(type: "INTEGER", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CharacterClasses", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CharacterRaces",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Description = table.Column<string>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    RaceType = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CharacterRaces", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Characters",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CharacterClassId = table.Column<int>(type: "INTEGER", nullable: true),
                    CharacterRaceId = table.Column<int>(type: "INTEGER", nullable: true),
                    Health = table.Column<int>(type: "INTEGER", nullable: false),
                    Level = table.Column<int>(type: "INTEGER", nullable: false),
                    Mana = table.Column<int>(type: "INTEGER", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Characters", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Characters_CharacterClasses_CharacterClassId",
                        column: x => x.CharacterClassId,
                        principalTable: "CharacterClasses",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Characters_CharacterRaces_CharacterRaceId",
                        column: x => x.CharacterRaceId,
                        principalTable: "CharacterRaces",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "RaceStatBonus",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    BonusValue = table.Column<int>(type: "INTEGER", nullable: false),
                    CharacterRaceId = table.Column<int>(type: "INTEGER", nullable: true),
                    IsSelectable = table.Column<bool>(type: "INTEGER", nullable: false),
                    RaceId = table.Column<int>(type: "INTEGER", nullable: false),
                    StatId = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RaceStatBonus", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RaceStatBonus_CharacterRaces_CharacterRaceId",
                        column: x => x.CharacterRaceId,
                        principalTable: "CharacterRaces",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_CharacterStats_CharacterId",
                table: "CharacterStats",
                column: "CharacterId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CharacterStatBonus_CharacterId",
                table: "CharacterStatBonus",
                column: "CharacterId");

            migrationBuilder.CreateIndex(
                name: "IX_Characters_CharacterClassId",
                table: "Characters",
                column: "CharacterClassId");

            migrationBuilder.CreateIndex(
                name: "IX_Characters_CharacterRaceId",
                table: "Characters",
                column: "CharacterRaceId");

            migrationBuilder.CreateIndex(
                name: "IX_RaceStatBonus_CharacterRaceId",
                table: "RaceStatBonus",
                column: "CharacterRaceId");

            migrationBuilder.AddForeignKey(
                name: "FK_CharacterStatBonus_Characters_CharacterId",
                table: "CharacterStatBonus",
                column: "CharacterId",
                principalTable: "Characters",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CharacterStats_Characters_CharacterId",
                table: "CharacterStats",
                column: "CharacterId",
                principalTable: "Characters",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_RacialModifiers_CharacterRaces_RaceId",
                table: "RacialModifiers",
                column: "RaceId",
                principalTable: "CharacterRaces",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
