using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ScholarisReborn.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class TermFormatAndSchoolLogo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AcademicYearStart",
                table: "Terms",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "PeriodNumber",
                table: "Terms",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<byte[]>(
                name: "Logo",
                table: "Schools",
                type: "bytea",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LogoContentType",
                table: "Schools",
                type: "character varying(128)",
                maxLength: 128,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AcademicYearStart",
                table: "Terms");

            migrationBuilder.DropColumn(
                name: "PeriodNumber",
                table: "Terms");

            migrationBuilder.DropColumn(
                name: "Logo",
                table: "Schools");

            migrationBuilder.DropColumn(
                name: "LogoContentType",
                table: "Schools");
        }
    }
}
