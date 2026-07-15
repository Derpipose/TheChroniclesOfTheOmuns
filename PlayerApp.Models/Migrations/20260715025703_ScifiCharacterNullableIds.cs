using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PlayerApp.Models.Migrations
{
    /// <inheritdoc />
    public partial class ScifiCharacterNullableIds : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ScifiCharacters_ScifiBackgrounds_ScifiBackgroundId",
                table: "ScifiCharacters");

            migrationBuilder.DropForeignKey(
                name: "FK_ScifiCharacters_ScifiClasses_ScifiClassId",
                table: "ScifiCharacters");

            migrationBuilder.DropForeignKey(
                name: "FK_ScifiCharacters_ScifiRaces_ScifiRaceId",
                table: "ScifiCharacters");

            migrationBuilder.DropForeignKey(
                name: "FK_ScifiCharacters_ScifiSubraces_ScifiSubraceId",
                table: "ScifiCharacters");

            migrationBuilder.AlterColumn<int>(
                name: "ScifiSubraceId",
                table: "ScifiCharacters",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<int>(
                name: "ScifiRaceId",
                table: "ScifiCharacters",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<int>(
                name: "ScifiClassId",
                table: "ScifiCharacters",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<int>(
                name: "ScifiBackgroundId",
                table: "ScifiCharacters",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddForeignKey(
                name: "FK_ScifiCharacters_ScifiBackgrounds_ScifiBackgroundId",
                table: "ScifiCharacters",
                column: "ScifiBackgroundId",
                principalTable: "ScifiBackgrounds",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ScifiCharacters_ScifiClasses_ScifiClassId",
                table: "ScifiCharacters",
                column: "ScifiClassId",
                principalTable: "ScifiClasses",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ScifiCharacters_ScifiRaces_ScifiRaceId",
                table: "ScifiCharacters",
                column: "ScifiRaceId",
                principalTable: "ScifiRaces",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ScifiCharacters_ScifiSubraces_ScifiSubraceId",
                table: "ScifiCharacters",
                column: "ScifiSubraceId",
                principalTable: "ScifiSubraces",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ScifiCharacters_ScifiBackgrounds_ScifiBackgroundId",
                table: "ScifiCharacters");

            migrationBuilder.DropForeignKey(
                name: "FK_ScifiCharacters_ScifiClasses_ScifiClassId",
                table: "ScifiCharacters");

            migrationBuilder.DropForeignKey(
                name: "FK_ScifiCharacters_ScifiRaces_ScifiRaceId",
                table: "ScifiCharacters");

            migrationBuilder.DropForeignKey(
                name: "FK_ScifiCharacters_ScifiSubraces_ScifiSubraceId",
                table: "ScifiCharacters");

            migrationBuilder.AlterColumn<int>(
                name: "ScifiSubraceId",
                table: "ScifiCharacters",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "ScifiRaceId",
                table: "ScifiCharacters",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "ScifiClassId",
                table: "ScifiCharacters",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "ScifiBackgroundId",
                table: "ScifiCharacters",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_ScifiCharacters_ScifiBackgrounds_ScifiBackgroundId",
                table: "ScifiCharacters",
                column: "ScifiBackgroundId",
                principalTable: "ScifiBackgrounds",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ScifiCharacters_ScifiClasses_ScifiClassId",
                table: "ScifiCharacters",
                column: "ScifiClassId",
                principalTable: "ScifiClasses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ScifiCharacters_ScifiRaces_ScifiRaceId",
                table: "ScifiCharacters",
                column: "ScifiRaceId",
                principalTable: "ScifiRaces",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ScifiCharacters_ScifiSubraces_ScifiSubraceId",
                table: "ScifiCharacters",
                column: "ScifiSubraceId",
                principalTable: "ScifiSubraces",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
