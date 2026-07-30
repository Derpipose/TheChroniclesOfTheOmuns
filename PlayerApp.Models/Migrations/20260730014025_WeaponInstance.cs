using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace PlayerApp.Models.Migrations
{
    /// <inheritdoc />
    public partial class WeaponInstance : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ScifiWeaponInstances",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ScifiWeaponId = table.Column<int>(type: "integer", nullable: false),
                    ScifiCharacterId = table.Column<int>(type: "integer", nullable: false),
                    CurrentAmmo = table.Column<int>(type: "integer", nullable: false),
                    DamageFormula = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    DamageType = table.Column<int>(type: "integer", nullable: false),
                    EnergyType = table.Column<int>(type: "integer", nullable: false),
                    RateOfFire = table.Column<int>(type: "integer", nullable: false),
                    AmmoCapacity = table.Column<int>(type: "integer", nullable: false),
                    ReloadTimeInActions = table.Column<int>(type: "integer", nullable: false),
                    CriticalRange = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    CriticalMultiplier = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ScifiWeaponInstances", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ScifiWeaponInstances_ScifiCharacters_ScifiCharacterId",
                        column: x => x.ScifiCharacterId,
                        principalTable: "ScifiCharacters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ScifiWeapons",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    DamageFormula = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    ModifierStat = table.Column<int>(type: "integer", nullable: false),
                    DamageType = table.Column<int>(type: "integer", nullable: false),
                    EnergyType = table.Column<int>(type: "integer", nullable: false),
                    RateOfFire = table.Column<int>(type: "integer", nullable: false),
                    AmmoCapacity = table.Column<int>(type: "integer", nullable: false),
                    ReloadTimeInActions = table.Column<int>(type: "integer", nullable: false),
                    CriticalRange = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    CriticalMultiplier = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    RangeCategory = table.Column<string>(type: "text", nullable: true),
                    CriticalFailRange = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ScifiWeapons", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ScifiWeaponInstances_ScifiCharacterId",
                table: "ScifiWeaponInstances",
                column: "ScifiCharacterId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ScifiWeaponInstances");

            migrationBuilder.DropTable(
                name: "ScifiWeapons");
        }
    }
}
