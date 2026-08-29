using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PlayerApp.Models.Migrations
{
    /// <inheritdoc />
    public partial class ScifiWeaponsandNotes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "RateOfFire",
                table: "ScifiWeapons",
                type: "text",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.DropColumn(
                name: "RangeCategory",
                table: "ScifiWeapons");

            migrationBuilder.AddColumn<int>(
                name: "RangeCategory",
                table: "ScifiWeapons",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.DropColumn(
                name: "CriticalRange",
                table: "ScifiWeapons");

            migrationBuilder.AddColumn<int>(
                name: "CriticalRange",
                table: "ScifiWeapons",
                type: "integer",
                maxLength: 10,
                nullable: false,
                defaultValue: 0);

            migrationBuilder.DropColumn(
                name: "CriticalMultiplier",
                table: "ScifiWeapons");

            migrationBuilder.AddColumn<int>(
                name: "CriticalMultiplier",
                table: "ScifiWeapons",
                type: "integer",
                maxLength: 10,
                nullable: false,
                defaultValue: 0);

            migrationBuilder.DropColumn(
                name: "CriticalFailRange",
                table: "ScifiWeapons");

            migrationBuilder.AddColumn<int>(
                name: "CriticalFailRange",
                table: "ScifiWeapons",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "AmmoType",
                table: "ScifiWeapons",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<string>(
                name: "RateOfFire",
                table: "ScifiWeaponInstances",
                type: "text",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.DropColumn(
                name: "CriticalRange",
                table: "ScifiWeaponInstances");

            migrationBuilder.AddColumn<int>(
                name: "CriticalRange",
                table: "ScifiWeaponInstances",
                type: "integer",
                maxLength: 10,
                nullable: false,
                defaultValue: 0);

            migrationBuilder.DropColumn(
                name: "CriticalMultiplier",
                table: "ScifiWeaponInstances");

            migrationBuilder.AddColumn<int>(
                name: "CriticalMultiplier",
                table: "ScifiWeaponInstances",
                type: "integer",
                maxLength: 10,
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "AmmoType",
                table: "ScifiWeaponInstances",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "CriticalFailRange",
                table: "ScifiWeaponInstances",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "Favorite",
                table: "ScifiWeaponInstances",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "ModifierStat",
                table: "ScifiWeaponInstances",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "ScifiWeaponInstances",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Nickname",
                table: "ScifiWeaponInstances",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "RangeCategory",
                table: "ScifiWeaponInstances",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Notes",
                table: "Characters",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AmmoType",
                table: "ScifiWeapons");

            migrationBuilder.DropColumn(
                name: "AmmoType",
                table: "ScifiWeaponInstances");

            migrationBuilder.DropColumn(
                name: "CriticalFailRange",
                table: "ScifiWeaponInstances");

            migrationBuilder.DropColumn(
                name: "Favorite",
                table: "ScifiWeaponInstances");

            migrationBuilder.DropColumn(
                name: "ModifierStat",
                table: "ScifiWeaponInstances");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "ScifiWeaponInstances");

            migrationBuilder.DropColumn(
                name: "Nickname",
                table: "ScifiWeaponInstances");

            migrationBuilder.DropColumn(
                name: "RangeCategory",
                table: "ScifiWeaponInstances");

            migrationBuilder.DropColumn(
                name: "Notes",
                table: "Characters");

            migrationBuilder.AlterColumn<int>(
                name: "RateOfFire",
                table: "ScifiWeapons",
                type: "integer",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "RangeCategory",
                table: "ScifiWeapons",
                type: "text",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<string>(
                name: "CriticalRange",
                table: "ScifiWeapons",
                type: "character varying(10)",
                maxLength: 10,
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer",
                oldMaxLength: 10);

            migrationBuilder.AlterColumn<string>(
                name: "CriticalMultiplier",
                table: "ScifiWeapons",
                type: "character varying(10)",
                maxLength: 10,
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer",
                oldMaxLength: 10);

            migrationBuilder.AlterColumn<string>(
                name: "CriticalFailRange",
                table: "ScifiWeapons",
                type: "text",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "RateOfFire",
                table: "ScifiWeaponInstances",
                type: "integer",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "CriticalRange",
                table: "ScifiWeaponInstances",
                type: "character varying(10)",
                maxLength: 10,
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer",
                oldMaxLength: 10);

            migrationBuilder.AlterColumn<string>(
                name: "CriticalMultiplier",
                table: "ScifiWeaponInstances",
                type: "character varying(10)",
                maxLength: 10,
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer",
                oldMaxLength: 10);
        }
    }
}
