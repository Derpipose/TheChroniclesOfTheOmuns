using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PlayerApp.Models.Migrations
{
    /// <inheritdoc />
    public partial class CharacterSettingEnumChange : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                ALTER TABLE ""Characters""
                ALTER COLUMN ""Setting"" TYPE integer
                USING (
                    CASE ""Setting""
                        WHEN 'Unregistered' THEN 0
                        WHEN 'Scifi' THEN 1
                        WHEN 'Fantasy' THEN 2
                        ELSE 0
                    END
                );
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Setting",
                table: "Characters",
                type: "text",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");
        }
    }
}
