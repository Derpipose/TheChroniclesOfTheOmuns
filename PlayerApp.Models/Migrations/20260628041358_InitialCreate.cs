using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace PlayerApp.Models.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Players",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    DisplayName = table.Column<string>(type: "text", nullable: false),
                    Email = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastLoginAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Players", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RaceStats",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Modifiers = table.Column<string>(type: "text", nullable: true),
                    FreeStatPicks = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RaceStats", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ScifiBackgrounds",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    Restricted = table.Column<bool>(type: "boolean", nullable: false),
                    HasClassOverlap = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ScifiBackgrounds", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ScifiClasses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    HitDice = table.Column<int>(type: "integer", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ScifiClasses", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ScifiRaces",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    SubType = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    IsStarterRace = table.Column<bool>(type: "boolean", nullable: false),
                    RaceStatsId = table.Column<int>(type: "integer", nullable: false),
                    Speed = table.Column<int>(type: "integer", nullable: false),
                    AdventuringAgeStart = table.Column<int>(type: "integer", nullable: false),
                    AgeMax = table.Column<int>(type: "integer", nullable: false),
                    IsPlayable = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ScifiRaces", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ScifiSubraces",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ScifiRaceId = table.Column<int>(type: "integer", nullable: false),
                    SubRaceName = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    RaceStatsId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ScifiSubraces", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ScifiTalents",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ScifiTalents", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AuthCode",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    PlayerId = table.Column<int>(type: "integer", nullable: false),
                    CodeHash = table.Column<string>(type: "text", nullable: false),
                    ExpiresAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Used = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuthCode", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AuthCode_Players_PlayerId",
                        column: x => x.PlayerId,
                        principalTable: "Players",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Characters",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    PlayerId = table.Column<int>(type: "integer", nullable: false),
                    Level = table.Column<int>(type: "integer", nullable: false),
                    Setting = table.Column<string>(type: "text", nullable: false),
                    MaxHealth = table.Column<int>(type: "integer", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Characters", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Characters_Players_PlayerId",
                        column: x => x.PlayerId,
                        principalTable: "Players",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ScifiBackgroundTalents",
                columns: table => new
                {
                    ScifiBackgroundId = table.Column<int>(type: "integer", nullable: false),
                    ScifiTalentId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ScifiBackgroundTalents", x => new { x.ScifiBackgroundId, x.ScifiTalentId });
                    table.ForeignKey(
                        name: "FK_ScifiBackgroundTalents_ScifiBackgrounds_ScifiBackgroundId",
                        column: x => x.ScifiBackgroundId,
                        principalTable: "ScifiBackgrounds",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ScifiBackgroundTalents_ScifiTalents_ScifiTalentId",
                        column: x => x.ScifiTalentId,
                        principalTable: "ScifiTalents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ScifiClassTalents",
                columns: table => new
                {
                    ScifiClassId = table.Column<int>(type: "integer", nullable: false),
                    ScifiTalentId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ScifiClassTalents", x => new { x.ScifiClassId, x.ScifiTalentId });
                    table.ForeignKey(
                        name: "FK_ScifiClassTalents_ScifiClasses_ScifiClassId",
                        column: x => x.ScifiClassId,
                        principalTable: "ScifiClasses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ScifiClassTalents_ScifiTalents_ScifiTalentId",
                        column: x => x.ScifiTalentId,
                        principalTable: "ScifiTalents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ScifiCharacters",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CharacterId = table.Column<int>(type: "integer", nullable: false),
                    ScifiRaceId = table.Column<int>(type: "integer", nullable: false),
                    ScifiSubraceId = table.Column<int>(type: "integer", nullable: false),
                    ScifiClassId = table.Column<int>(type: "integer", nullable: false),
                    ScifiBackgroundId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ScifiCharacters", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ScifiCharacters_Characters_CharacterId",
                        column: x => x.CharacterId,
                        principalTable: "Characters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ScifiCharacters_ScifiBackgrounds_ScifiBackgroundId",
                        column: x => x.ScifiBackgroundId,
                        principalTable: "ScifiBackgrounds",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ScifiCharacters_ScifiClasses_ScifiClassId",
                        column: x => x.ScifiClassId,
                        principalTable: "ScifiClasses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ScifiCharacters_ScifiRaces_ScifiRaceId",
                        column: x => x.ScifiRaceId,
                        principalTable: "ScifiRaces",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ScifiCharacters_ScifiSubraces_ScifiSubraceId",
                        column: x => x.ScifiSubraceId,
                        principalTable: "ScifiSubraces",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AuthCode_PlayerId",
                table: "AuthCode",
                column: "PlayerId");

            migrationBuilder.CreateIndex(
                name: "IX_Characters_PlayerId",
                table: "Characters",
                column: "PlayerId");

            migrationBuilder.CreateIndex(
                name: "IX_ScifiBackgroundTalents_ScifiTalentId",
                table: "ScifiBackgroundTalents",
                column: "ScifiTalentId");

            migrationBuilder.CreateIndex(
                name: "IX_ScifiCharacters_CharacterId",
                table: "ScifiCharacters",
                column: "CharacterId");

            migrationBuilder.CreateIndex(
                name: "IX_ScifiCharacters_ScifiBackgroundId",
                table: "ScifiCharacters",
                column: "ScifiBackgroundId");

            migrationBuilder.CreateIndex(
                name: "IX_ScifiCharacters_ScifiClassId",
                table: "ScifiCharacters",
                column: "ScifiClassId");

            migrationBuilder.CreateIndex(
                name: "IX_ScifiCharacters_ScifiRaceId",
                table: "ScifiCharacters",
                column: "ScifiRaceId");

            migrationBuilder.CreateIndex(
                name: "IX_ScifiCharacters_ScifiSubraceId",
                table: "ScifiCharacters",
                column: "ScifiSubraceId");

            migrationBuilder.CreateIndex(
                name: "IX_ScifiClassTalents_ScifiTalentId",
                table: "ScifiClassTalents",
                column: "ScifiTalentId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AuthCode");

            migrationBuilder.DropTable(
                name: "RaceStats");

            migrationBuilder.DropTable(
                name: "ScifiBackgroundTalents");

            migrationBuilder.DropTable(
                name: "ScifiCharacters");

            migrationBuilder.DropTable(
                name: "ScifiClassTalents");

            migrationBuilder.DropTable(
                name: "Characters");

            migrationBuilder.DropTable(
                name: "ScifiBackgrounds");

            migrationBuilder.DropTable(
                name: "ScifiRaces");

            migrationBuilder.DropTable(
                name: "ScifiSubraces");

            migrationBuilder.DropTable(
                name: "ScifiClasses");

            migrationBuilder.DropTable(
                name: "ScifiTalents");

            migrationBuilder.DropTable(
                name: "Players");
        }
    }
}
