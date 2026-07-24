using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PlayerApp.Models.Migrations
{
    /// <inheritdoc />
    public partial class NullableRaceIdsRace : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_ScifiRaces_RaceStatsId",
                table: "ScifiRaces",
                column: "RaceStatsId");

            migrationBuilder.AddForeignKey(
                name: "FK_ScifiRaces_RaceStats_RaceStatsId",
                table: "ScifiRaces",
                column: "RaceStatsId",
                principalTable: "RaceStats",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ScifiRaces_RaceStats_RaceStatsId",
                table: "ScifiRaces");

            migrationBuilder.DropIndex(
                name: "IX_ScifiRaces_RaceStatsId",
                table: "ScifiRaces");
        }
    }
}
