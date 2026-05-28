using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PlayerAppBlazor.Migrations;

/// <inheritdoc />
public partial class PendingChanges : Migration {
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder) {
        migrationBuilder.CreateTable(
            name: "CharacterStatBonus",
            columns: table => new {
                Id = table.Column<int>(type: "INTEGER", nullable: false)
                    .Annotation("Sqlite:Autoincrement", true),
                CharacterId = table.Column<int>(type: "INTEGER", nullable: false),
                BonusValue = table.Column<int>(type: "INTEGER", nullable: false),
                BonusSource = table.Column<string>(type: "TEXT", nullable: false),
                StatId = table.Column<int>(type: "INTEGER", nullable: true),
                IsSelectable = table.Column<bool>(type: "INTEGER", nullable: false)
            },
            constraints: table => {
                table.PrimaryKey("PK_CharacterStatBonus", x => x.Id);
                table.ForeignKey(
                    name: "FK_CharacterStatBonus_Characters_CharacterId",
                    column: x => x.CharacterId,
                    principalTable: "Characters",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateIndex(
            name: "IX_CharacterStatBonus_CharacterId",
            table: "CharacterStatBonus",
            column: "CharacterId");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder) {
        migrationBuilder.DropTable(
            name: "CharacterStatBonus");
    }
}
