using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PlayerApp.Models.Migrations
{
    /// <inheritdoc />
    public partial class RaceStatsToSubrace : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_ScifiSubraces_RaceStatsId",
                table: "ScifiSubraces",
                column: "RaceStatsId");

            migrationBuilder.CreateIndex(
                name: "IX_ScifiSubraces_ScifiRaceId",
                table: "ScifiSubraces",
                column: "ScifiRaceId");

            migrationBuilder.AddForeignKey(
                name: "FK_ScifiSubraces_RaceStats_RaceStatsId",
                table: "ScifiSubraces",
                column: "RaceStatsId",
                principalTable: "RaceStats",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ScifiSubraces_ScifiRaces_ScifiRaceId",
                table: "ScifiSubraces",
                column: "ScifiRaceId",
                principalTable: "ScifiRaces",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ScifiSubraces_RaceStats_RaceStatsId",
                table: "ScifiSubraces");

            migrationBuilder.DropForeignKey(
                name: "FK_ScifiSubraces_ScifiRaces_ScifiRaceId",
                table: "ScifiSubraces");

            migrationBuilder.DropIndex(
                name: "IX_ScifiSubraces_RaceStatsId",
                table: "ScifiSubraces");

            migrationBuilder.DropIndex(
                name: "IX_ScifiSubraces_ScifiRaceId",
                table: "ScifiSubraces");
        }
    }
}
