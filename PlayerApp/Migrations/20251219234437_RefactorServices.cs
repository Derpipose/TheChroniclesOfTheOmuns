using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace PlayerApp.Migrations;

/// <inheritdoc />
public partial class RefactorServices : Migration {
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder) {
        migrationBuilder.AddColumn<string>(
            name: "ClassType",
            table: "CharacterClass",
            type: "nvarchar(max)",
            nullable: false,
            defaultValue: "");

        migrationBuilder.AddColumn<int>(
            name: "HitDiceId",
            table: "CharacterClass",
            type: "int",
            nullable: false,
            defaultValue: 0);

        migrationBuilder.AddColumn<int>(
            name: "ManaDiceId",
            table: "CharacterClass",
            type: "int",
            nullable: false,
            defaultValue: 0);

        migrationBuilder.AddColumn<int>(
            name: "Health",
            table: "Character",
            type: "int",
            nullable: false,
            defaultValue: 0);

        migrationBuilder.AddColumn<int>(
            name: "Mana",
            table: "Character",
            type: "int",
            nullable: false,
            defaultValue: 0);

        migrationBuilder.CreateTable(
            name: "DiceTypes",
            columns: table => new {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                Sides = table.Column<int>(type: "int", nullable: false)
            },
            constraints: table => {
                table.PrimaryKey("PK_DiceTypes", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "Modifiers",
            columns: table => new {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                Type = table.Column<int>(type: "int", nullable: false),
                Value = table.Column<int>(type: "int", nullable: false),
                Description = table.Column<string>(type: "nvarchar(max)", nullable: true)
            },
            constraints: table => {
                table.PrimaryKey("PK_Modifiers", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "RacialModifiers",
            columns: table => new {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                RaceId = table.Column<int>(type: "int", nullable: false),
                ModifierId = table.Column<int>(type: "int", nullable: false)
            },
            constraints: table => {
                table.PrimaryKey("PK_RacialModifiers", x => x.Id);
                table.ForeignKey(
                    name: "FK_RacialModifiers_CharacterRace_RaceId",
                    column: x => x.RaceId,
                    principalTable: "CharacterRace",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    name: "FK_RacialModifiers_Modifiers_ModifierId",
                    column: x => x.ModifierId,
                    principalTable: "Modifiers",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.InsertData(
            table: "DiceTypes",
            columns: new[] { "Id", "Name", "Sides" },
            values: new object[,]
            {
                { 1, "D4", 4 },
                { 2, "D6", 6 },
                { 3, "D8", 8 },
                { 4, "D10", 10 },
                { 5, "D12", 12 },
                { 6, "D20", 20 }
            });

        migrationBuilder.CreateIndex(
            name: "IX_CharacterClass_HitDiceId",
            table: "CharacterClass",
            column: "HitDiceId");

        migrationBuilder.CreateIndex(
            name: "IX_CharacterClass_ManaDiceId",
            table: "CharacterClass",
            column: "ManaDiceId");

        migrationBuilder.CreateIndex(
            name: "IX_RacialModifiers_ModifierId",
            table: "RacialModifiers",
            column: "ModifierId");

        migrationBuilder.CreateIndex(
            name: "IX_RacialModifiers_RaceId",
            table: "RacialModifiers",
            column: "RaceId");

        migrationBuilder.AddForeignKey(
            name: "FK_CharacterClass_DiceTypes_HitDiceId",
            table: "CharacterClass",
            column: "HitDiceId",
            principalTable: "DiceTypes",
            principalColumn: "Id");

        migrationBuilder.AddForeignKey(
            name: "FK_CharacterClass_DiceTypes_ManaDiceId",
            table: "CharacterClass",
            column: "ManaDiceId",
            principalTable: "DiceTypes",
            principalColumn: "Id");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder) {
        migrationBuilder.DropForeignKey(
            name: "FK_CharacterClass_DiceTypes_HitDiceId",
            table: "CharacterClass");

        migrationBuilder.DropForeignKey(
            name: "FK_CharacterClass_DiceTypes_ManaDiceId",
            table: "CharacterClass");

        migrationBuilder.DropTable(
            name: "DiceTypes");

        migrationBuilder.DropTable(
            name: "RacialModifiers");

        migrationBuilder.DropTable(
            name: "Modifiers");

        migrationBuilder.DropIndex(
            name: "IX_CharacterClass_HitDiceId",
            table: "CharacterClass");

        migrationBuilder.DropIndex(
            name: "IX_CharacterClass_ManaDiceId",
            table: "CharacterClass");

        migrationBuilder.DropColumn(
            name: "ClassType",
            table: "CharacterClass");

        migrationBuilder.DropColumn(
            name: "HitDiceId",
            table: "CharacterClass");

        migrationBuilder.DropColumn(
            name: "ManaDiceId",
            table: "CharacterClass");

        migrationBuilder.DropColumn(
            name: "Health",
            table: "Character");

        migrationBuilder.DropColumn(
            name: "Mana",
            table: "Character");
    }
}
