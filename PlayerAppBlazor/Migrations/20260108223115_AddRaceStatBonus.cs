using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PlayerAppBlazor.Migrations;

/// <inheritdoc />
public partial class AddRaceStatBonus : Migration {
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder) {
        migrationBuilder.CreateTable(
            name: "RaceStatBonus",
            columns: table => new {
                Id = table.Column<int>(type: "INTEGER", nullable: false)
                    .Annotation("Sqlite:Autoincrement", true),
                RaceId = table.Column<int>(type: "INTEGER", nullable: false),
                BonusValue = table.Column<int>(type: "INTEGER", nullable: false),
                StatId = table.Column<int>(type: "INTEGER", nullable: true),
                IsSelectable = table.Column<bool>(type: "INTEGER", nullable: false),
                CharacterRaceId = table.Column<int>(type: "INTEGER", nullable: true)
            },
            constraints: table => {
                table.PrimaryKey("PK_RaceStatBonus", x => x.Id);
                table.ForeignKey(
                    name: "FK_RaceStatBonus_CharacterRaces_CharacterRaceId",
                    column: x => x.CharacterRaceId,
                    principalTable: "CharacterRaces",
                    principalColumn: "Id");
            });

        migrationBuilder.CreateIndex(
            name: "IX_RaceStatBonus_CharacterRaceId",
            table: "RaceStatBonus",
            column: "CharacterRaceId");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder) {
        migrationBuilder.DropTable(
            name: "RaceStatBonus");
    }
}
