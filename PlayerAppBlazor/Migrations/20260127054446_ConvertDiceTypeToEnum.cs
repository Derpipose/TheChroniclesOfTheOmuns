using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace PlayerAppBlazor.Migrations
{
    /// <inheritdoc />
    public partial class ConvertDiceTypeToEnum : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("PRAGMA foreign_keys = 0;");
            
            migrationBuilder.DropForeignKey(
                name: "FK_CharacterClasses_DiceTypes_HitDiceId",
                table: "CharacterClasses");

            migrationBuilder.DropForeignKey(
                name: "FK_CharacterClasses_DiceTypes_ManaDiceId",
                table: "CharacterClasses");

            migrationBuilder.DropIndex(
                name: "IX_CharacterClasses_HitDiceId",
                table: "CharacterClasses");

            migrationBuilder.DropIndex(
                name: "IX_CharacterClasses_ManaDiceId",
                table: "CharacterClasses");

            migrationBuilder.DropTable(
                name: "DiceTypes");

            migrationBuilder.RenameColumn(
                name: "ManaDiceId",
                table: "CharacterClasses",
                newName: "ManaDice");

            migrationBuilder.RenameColumn(
                name: "HitDiceId",
                table: "CharacterClasses",
                newName: "HitDice");

            migrationBuilder.Sql("PRAGMA foreign_keys = 1;");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ManaDice",
                table: "CharacterClasses",
                newName: "ManaDiceId");

            migrationBuilder.RenameColumn(
                name: "HitDice",
                table: "CharacterClasses",
                newName: "HitDiceId");

            migrationBuilder.CreateTable(
                name: "DiceTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Sides = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DiceTypes", x => x.Id);
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
                name: "IX_CharacterClasses_HitDiceId",
                table: "CharacterClasses",
                column: "HitDiceId");

            migrationBuilder.CreateIndex(
                name: "IX_CharacterClasses_ManaDiceId",
                table: "CharacterClasses",
                column: "ManaDiceId");

            migrationBuilder.AddForeignKey(
                name: "FK_CharacterClasses_DiceTypes_HitDiceId",
                table: "CharacterClasses",
                column: "HitDiceId",
                principalTable: "DiceTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CharacterClasses_DiceTypes_ManaDiceId",
                table: "CharacterClasses",
                column: "ManaDiceId",
                principalTable: "DiceTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
