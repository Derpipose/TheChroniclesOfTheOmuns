using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PlayerApp.Models.Migrations
{
    /// <inheritdoc />
    public partial class AddAuthCodes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AuthCode_Players_PlayerId",
                table: "AuthCode");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AuthCode",
                table: "AuthCode");

            migrationBuilder.RenameTable(
                name: "AuthCode",
                newName: "AuthCodes");

            migrationBuilder.RenameIndex(
                name: "IX_AuthCode_PlayerId",
                table: "AuthCodes",
                newName: "IX_AuthCodes_PlayerId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AuthCodes",
                table: "AuthCodes",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AuthCodes_Players_PlayerId",
                table: "AuthCodes",
                column: "PlayerId",
                principalTable: "Players",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AuthCodes_Players_PlayerId",
                table: "AuthCodes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AuthCodes",
                table: "AuthCodes");

            migrationBuilder.RenameTable(
                name: "AuthCodes",
                newName: "AuthCode");

            migrationBuilder.RenameIndex(
                name: "IX_AuthCodes_PlayerId",
                table: "AuthCode",
                newName: "IX_AuthCode_PlayerId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AuthCode",
                table: "AuthCode",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AuthCode_Players_PlayerId",
                table: "AuthCode",
                column: "PlayerId",
                principalTable: "Players",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
